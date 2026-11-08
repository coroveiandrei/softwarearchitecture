param location string = resourceGroup().location

param sqlServers array 

@secure()
param sqlAdministratorPassword string

param resourceTags object = {
  DeploymentType: 'Bicep'
}


module databaseServerModule 'modules/sqlDatabase.bicep' = [ for sqlServerInstance in sqlServers: {
  name: '${sqlServerInstance.name}-sqlserver-module'
  params: {
    location: location
    servername: sqlServerInstance.name
    resourceTags: resourceTags
    databaseServerName: sqlServerInstance.name
    databaseName:  sqlServerInstance.database
    username: sqlServerInstance.username
    password: sqlAdministratorPassword
  }
}]
