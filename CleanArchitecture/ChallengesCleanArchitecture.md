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
Deploy this architecture on an Azure stack and run it on Azure. What services from the ones created previously would you need?

1. Go to Azure Portal and grab the connection string of Azure SQL DB created in the last lab. If you don't have it, you can always redeploy the template

The connection string should look like this:
````Server=tcp:[CHANGEMESERVER].database.windows.net,1433;Initial Catalog=arcacsqldb;Persist Security Info=False;User ID=andrei;Password=[CHANGEME];MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;````

2. In SQL Server also make sure firewall rules are open for your ip address
3. Go to appsettings.json and replace ToDoDatabase with the connection string above
4. Provision an AppService in Azure 

Enable Azure services access to the database from network configuration.
