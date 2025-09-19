 #\create-table-storage.ps1 -ResourceGroupName "blog" -StorageAccountName "andreicoroveistor"
param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$StorageAccountName
)

$ErrorActionPreference = "Stop"
$TableName = "Enrollments"

# Ensure we're logged into Azure
$account = az account show 2>$null | ConvertFrom-Json
if (-not $account) {
    Write-Error "Not logged into Azure. Please run 'az login' first."
    exit 1
}

# Get storage account key
Write-Host "Getting storage account key..."
$keys = az storage account keys list --resource-group $ResourceGroupName --account-name $StorageAccountName | ConvertFrom-Json
$accountKey = $keys[0].value

# Create table
Write-Host "Creating table '$TableName'..."
az storage table create `
    --name $TableName `
    --account-name $StorageAccountName `
    --account-key $accountKey

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to create table"
    exit 1
}

# Helper functions for generating random data
function Get-RandomName {
    $firstNames = @("John", "Jane", "Michael", "Sarah", "David", "Emma", "James", "Emily", "William", "Olivia",
                   "Daniel", "Sophia", "Matthew", "Ava", "Joseph", "Isabella", "Christopher", "Mia", "Andrew", "Charlotte")
    $lastNames = @("Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
                  "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin")
    
    $firstName = Get-Random -InputObject $firstNames
    $lastName = Get-Random -InputObject $lastNames
    return "$firstName $lastName"
}

function Get-RandomDateOfBirth {
    # Generate date between 1990 and 2005 (for students aged ~20-35)
    $start = Get-Date -Year 1990 -Month 1 -Day 1
    $end = Get-Date -Year 2005 -Month 12 -Day 31
    $range = New-TimeSpan -Start $start -End $end
    $randomDays = Get-Random -Minimum 0 -Maximum $range.Days
    return $start.AddDays($randomDays).ToString("yyyy-MM-dd")
}

$subjects = @("Mathematics", "Computer Science", "English")
$totalEnrollments = 10000
$numberOfJobs = 100
$recordsPerJob = [math]::Ceiling($totalEnrollments / $numberOfJobs)

Write-Host "Creating $totalEnrollments enrollments using $numberOfJobs parallel jobs..."
$startTime = Get-Date

# Create a script block for the insert operation
$insertJob = {
    param($resourceGroup, $storageAccount, $tableName, $accountKey, $startIndex, $endIndex, $subjects)
    
    # Helper functions (need to be defined inside the job)
    function Get-RandomName {
        $firstNames = @("John", "Jane", "Michael", "Sarah", "David", "Emma", "James", "Emily", "William", "Olivia",
                       "Daniel", "Sophia", "Matthew", "Ava", "Joseph", "Isabella", "Christopher", "Mia", "Andrew", "Charlotte")
        $lastNames = @("Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
                      "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin")
        
        $firstName = Get-Random -InputObject $firstNames
        $lastName = Get-Random -InputObject $lastNames
        return "$firstName $lastName"
    }

    function Get-RandomDateOfBirth {
        $start = Get-Date -Year 1990 -Month 1 -Day 1
        $end = Get-Date -Year 2005 -Month 12 -Day 31
        $range = New-TimeSpan -Start $start -End $end
        $randomDays = Get-Random -Minimum 0 -Maximum $range.Days
        return $start.AddDays($randomDays).ToString("yyyy-MM-dd")
    }

    $processedCount = 0
    $errors = 0
    
    for ($i = $startIndex; $i -lt $endIndex; $i++) {
        $name = Get-RandomName
        $partitionKey = Get-Random -InputObject $subjects
        $rowKey = $name.Replace(" ", "_") + "_" + (New-Guid).ToString("N").Substring(0, 8)
        $dateOfBirth = Get-RandomDateOfBirth
        $enrollmentDate = (Get-Date).ToString("yyyy-MM-dd")
        
        $result = az storage entity insert `
            --table-name $tableName `
            --account-name $storageAccount `
            --account-key $accountKey `
            --if-exists fail `
            --entity PartitionKey=$partitionKey RowKey=$rowKey Name=$name DateOfBirth=$dateOfBirth EnrollmentDate=$enrollmentDate 2>&1

        if ($LASTEXITCODE -ne 0) {
            $errors++
        } else {
            $processedCount++
        }
    }
    
    return @{
        ProcessedCount = $processedCount
        Errors = $errors
        StartIndex = $startIndex
        EndIndex = $endIndex
    }
}

# Start parallel jobs
$jobs = @()
for ($i = 0; $i -lt $numberOfJobs; $i++) {
    $startIndex = $i * $recordsPerJob
    $endIndex = [Math]::Min(($i + 1) * $recordsPerJob, $totalEnrollments)
    
    Write-Host "Starting job $($i + 1) for records $startIndex to $endIndex..."
    
    $jobs += Start-Job -ScriptBlock $insertJob -ArgumentList $ResourceGroupName, $StorageAccountName, $TableName, $accountKey, $startIndex, $endIndex, $subjects
}

# Monitor jobs and show progress
$completed = 0
$totalProcessed = 0
$totalErrors = 0

while ($completed -lt $numberOfJobs) {
    $completed = 0
    $totalProcessed = 0
    $totalErrors = 0
    
    foreach ($job in $jobs) {
        $state = $job.State
        if ($state -eq "Completed") {
            $completed++
            $result = Receive-Job -Job $job -Keep
            $totalProcessed += $result.ProcessedCount
            $totalErrors += $result.Errors
        }
    }
    
    $elapsedTime = (Get-Date) - $startTime
    $recordsPerSecond = if ($elapsedTime.TotalSeconds -gt 0) { [math]::Round($totalProcessed / $elapsedTime.TotalSeconds, 1) } else { 0 }
    Write-Host "Jobs completed: $completed/$numberOfJobs, Records processed: $totalProcessed, Errors: $totalErrors, Rate: $recordsPerSecond records/second"
    
    if ($completed -lt $numberOfJobs) {
        Start-Sleep -Seconds 5
    }
}

# Clean up jobs
$jobs | Remove-Job

$totalTime = (Get-Date) - $startTime
$finalRate = [math]::Round($totalProcessed / $totalTime.TotalSeconds, 1)
Write-Host "`nAll jobs completed!"
Write-Host "Total records inserted: $totalProcessed"
Write-Host "Total errors: $totalErrors"
Write-Host "Total time: $([math]::Round($totalTime.TotalSeconds, 1)) seconds"
Write-Host "Average insertion rate: $finalRate records/second"

for ($i = 0; $i -lt 0; $i++) {  # This is just to replace the old for loop while keeping the indentation
    $name = Get-RandomName
    $partitionKey = Get-Random -InputObject $subjects
    $rowKey = $name.Replace(" ", "_") + "_" + (New-Guid).ToString("N").Substring(0, 8)
    $dateOfBirth = Get-RandomDateOfBirth
    $enrollmentDate = (Get-Date).ToString("yyyy-MM-dd")
    
    # Insert single entity
    $result = az storage entity insert `
        --table-name $TableName `
        --account-name $StorageAccountName `
        --account-key $accountKey `
        --if-exists fail `
        --entity PartitionKey=$partitionKey RowKey=$rowKey Name=$name DateOfBirth=$dateOfBirth EnrollmentDate=$enrollmentDate 2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to insert entity: $result"
        continue  # Skip this record but continue with others
    }

    $processedCount++
    if ($processedCount % 100 -eq 0) {
        $elapsedTime = (Get-Date) - $startTime
        $recordsPerSecond = [math]::Round($processedCount / $elapsedTime.TotalSeconds, 1)
        Write-Host "Processed $processedCount enrollments... ($recordsPerSecond records/second)"
    }
}

$totalTime = (Get-Date) - $startTime
$finalRate = [math]::Round($processedCount / $totalTime.TotalSeconds, 1)
Write-Host "Completed! Total records inserted: $processedCount"
Write-Host "Total time: $([math]::Round($totalTime.TotalSeconds, 1)) seconds"
Write-Host "Average insertion rate: $finalRate records/second"

# Cleanup
Remove-Item $tempFile -ErrorAction SilentlyContinue

$totalTime = (Get-Date) - $startTime
$finalRate = [math]::Round($processedCount / $totalTime.TotalSeconds, 1)
Write-Host "Completed! Total records inserted: $processedCount"
Write-Host "Total time: $([math]::Round($totalTime.TotalSeconds, 1)) seconds"
Write-Host "Average insertion rate: $finalRate records/second"


Write-Host "Completed! Total records inserted: $processedCount"
