param(
    [Parameter(Mandatory=$true)]
    [string]$UserEmail,
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "northeurope"
)

# Extract resource group name from email (everything before the @) and sanitize it
$ResourceGroupName = "rg-" + ($UserEmail.Split("@")[0].ToLower() -replace '[^a-z0-9]', '-')

# Ensure we're logged into Azure
$account = az account show 2>$null | ConvertFrom-Json
if (-not $account) {
    Write-Error "Not logged into Azure. Please run 'az login' first."
    exit 1
}

$subscriptionId = $account.id
$tenantId = $account.tenantId

# Invite the user to the tenant
Write-Host "Inviting user '$UserEmail' to the Azure AD tenant..."
Write-Host "Display name will be: '$displayName'"

# First check if user already exists
Write-Host "Checking if user already exists in the tenant..."
$existingUser = az ad user list --filter "mail eq '$UserEmail' or userPrincipalName eq '$UserEmail'" 2>&1
$userId = $null

if ($existingUser -and ($existingUserJson = $existingUser | ConvertFrom-Json) -and $existingUserJson.Length -gt 0) {
    Write-Host "User already exists in the tenant, using existing user..."
    $userId = $existingUserJson[0].id
    Write-Host "Found user ID: $userId"
} else {
    Write-Host "User not found in tenant, proceeding with invitation..."
    $displayName = $UserEmail.Split("@")[0] -replace '\.', ' ' # Convert john.doe to "john doe"
    $displayName = (Get-Culture).TextInfo.ToTitleCase($displayName) # Convert to "John Doe"
    
    Write-Host "Sending invitation command..."
    Write-Host "Tenant ID: $tenantId"
    $inviteError = $null
    $invitation = az rest --method POST --uri "https://graph.microsoft.com/v1.0/invitations" --body "{\""invitedUserEmailAddress\"":\""$UserEmail\"",\""inviteRedirectUrl\"":\""https://portal.azure.com\"",\""sendInvitationMessage\"":true}" 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Invitation command failed with exit code: $LASTEXITCODE"
        Write-Host "Error details:"
        Write-Host $invitation
        Write-Error "Failed to invite user. Please ensure the email address is valid and you have sufficient permissions."
        exit 1
    }
    
    try {
        $invitationJson = $invitation | ConvertFrom-Json
        Write-Host "Invitation sent successfully. Invitation details:"
        Write-Host "Invited user ID: $($invitationJson.invitedUser.id)"
        $userId = $invitationJson.invitedUser.id
    } catch {
        Write-Host "Warning: Could not parse invitation response as JSON. Raw response:"
        Write-Host $invitation
        exit 1
    }
}

Write-Host "User invitation sent successfully to $UserEmail"

# Create the resource group
Write-Host "Creating resource group '$ResourceGroupName' in location '$Location'..."
az group create --name $ResourceGroupName --location $Location

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to create resource group"
    exit 1
}

# Verify we have a user ID before proceeding
if (-not $userId) {
    Write-Error "No user ID found. This is unexpected - please check the Azure AD user status."
    exit 1
}

# Need to wait a bit for the invitation to propagate
Write-Host "Waiting for the user to be available in Azure AD..."
Start-Sleep -Seconds 15

# Get the user principal name (which is different for guest users)
Write-Host "Getting user principal name..."
$userPrincipal = az ad user show --id $userId | ConvertFrom-Json
if (-not $userPrincipal) {
    Write-Error "Could not find user in Azure AD. Please try again in a few moments."
    exit 1
}

# Assign Contributor role to the user
$userPrincipalName = $userPrincipal.userPrincipalName
Write-Host "Assigning Contributor role to '$userPrincipalName' (ID: $userId) on resource group '$ResourceGroupName'..."
az role assignment create `
    --assignee $userPrincipalName `
    --role Contributor `
    --scope "/subscriptions/$subscriptionId/resourceGroups/$ResourceGroupName" 

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to assign role. Please ensure the email address is valid and belongs to a user in your Azure AD."
    exit 1
}

Write-Host "Successfully created resource group and assigned Contributor access to $UserEmail"
