## Step 1 Create Azure Subcription

If you don't have an active Azure Subscription, you can create one at:
https://azure.microsoft.com/en-us/free/

Connect to portal.azure.com and create a new resource group called rg_arc_[yournameinitials]

## Step 2 Deploy Storage from Bicep template

1. Open up visual studio code
2. Open up folder in vs code C:\work\softwarearchforcloud\infracode
3. In storageparams.json, line 8, replace the name of the storage account with: arc[yourinitials]stor 
4. In VSCode terminal, run the commands:

az login

az deployment group what-if --resource-group  rg_arc_[yournameinitials] --template-file 1storage.bicep --parameters .\config\storageparams.json
This command will only show you what happens in case of a deployment

Now create the actual resources

az deployment group create  --resource-group  rg_arc_[yournameinitials] --template-file 1storage.bicep --parameters .\config\storageparams.json


## SQL administrator password

The SQL exercise accepts the password through the secure `sqlAdministratorPassword` parameter.
Create `config/sql-password.local.json` with the following content and replace the placeholder locally.

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "sqlAdministratorPassword": { "value": "REPLACE_LOCALLY" }
  }
}
```

Git ignores this local file. Set the resource names in `config/sqlserverparams.json`, then preview the deployment.

```powershell
az deployment group what-if --resource-group <your-resource-group> --template-file 4sqlserver.bicep --parameters ./config/sqlserverparams.json ./config/sql-password.local.json
```

After reviewing the preview, replace `what-if` with `create` to deploy.
