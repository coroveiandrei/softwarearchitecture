# Clean Architecture Challenges

### Map & start the solution:
1. Make sure Docker Desktop is started, then navigate to CleanArchitecture\infra folder and run **start-all.ps1**. This should start a local database from a Docker container.
2. Open up VS Code with .net core 8 installed and build the project 
      
   ````dotnet build````
   
   ````dotnet run --project CleanArc.WebUI```` 
   
4. Open up the app:
   ````https://localhost:2001/swagger/index.html````
5. Try the login endpoint with user: **admin** and password: **P@ssw0rd**
6. Try to add a new ToDo and then list it. If it succeeds, the project is fully configured 

### Chalenge #0
Inspect the solution and investigate the project structure and dependencies between them.
Determine where the API call lands initially and how it goes through the onion layers to the business logic and infra layer? Try to draw a diagram if you find it easier.

### Chalenge #1 
The app's end users just reported a bug: They cannot see the description after creating a to-do. 
Fix the bug

### Challenge #2
The end users want to receive an email when they add a new Todo. For the sake of simplicity, you can just call a webhook instead of sending emails. The service INotificationService is already in place, but something is wrong. Can you spot what it is? 

### Challenge #3
Your manager has decided that SQL is too expensive and would like to migrate the database to Azure Table Storage, which he believes is less expensive. You have been tasked with conducting an initial analysis and estimating the changes required for this migration. What classes or projects would be involved in this migration


### Challenge #4
1. Deploy this architecture on an Azure stack and run it on Azure. What services from the ones created previously would you need?
Provision the Appservice and Sql database

````az deployment group create --resource-group  rg-andrei-corovei --template-file 3appservice.bicep --parameters .\config\appserviceparams.json````

````az deployment group create --resource-group  rg-andrei-corovei --template-file 4sqlserver.bicep --parameters .\config\sqlserverparams.json````

2. Go to Azure Portal and grab the connection string of Azure SQL DB created.

The connection string should look like this:
````Server=tcp:[CHANGEMESERVER].database.windows.net,1433;Initial Catalog=arcacsqldb;Persist Security Info=False;User ID=andrei;Password=[CHANGEME];MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;````

3. In SQL Server also make sure firewall rules are open for your ip address
4. Go to appsettings.json and replace ToDoDatabase with the connection string above


````
# 1. Navigate to project folder
cd C:\code\softwarearchitecture\CleanArchitecture\CleanArc\CleanArc.WebUI
# 2. Publish the .NET app (run from your project directory)
dotnet publish -c Release
# 3. Create a zip archive of the publish output
Compress-Archive -Path C:\code\softwarearchitecture\CleanArchitecture\CleanArc\CleanArc.WebUI\bin\Release\net8.0\publish\* -DestinationPath C:\code\softwarearchitecture\CleanArchitecture\CleanArc\CleanArc.WebUI\publish.zip
# 4. Set env variable
az webapp config appsettings set --resource-group [RGNAME] --name [APPSERVICENAME] --settings ASPNETCORE_ENVIRONMENT=Development
# 5. Deploy the zip to Azure Web App
az webapp deploy --resource-group [RGNAME] --name [APPSERVICENAME] --src-path C:\code\softwarearchitecture\CleanArchitecture\CleanArc\CleanArc.WebUI\publish.zip --type zip
````
Remember to type /swagger at the end of the URL!!!

