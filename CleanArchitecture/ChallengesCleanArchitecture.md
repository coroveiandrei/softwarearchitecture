# Clean Architecture Challenges

### Map & start the solution:
1. Clone the repo at https://github.com/coroveiandrei/softwarearchitecture
2. Make sure Docker Destkop is started, then navigate to CleanArchitecture\infra folder and run **start-all.ps1**. This should start a local database from a Docker container.
3. Open up Visual Studio Community 2022 / VS Code with .net core 8 installed and build the project 
4. From Visual Studio: Set CleanArc.WebUI as a startup project and run the project

   From VSCode: run

   dotnet build

   dotnet run --project CleanArc.WebUI
   
5. At this point a browser windows should popup showing swagger page. If not, please open up a browser and add /swagger in the starting URL
6. Try the login endpoint with user: **admin** and password: **P@ssw0rd**
7. Try to add a new ToDo and then list it. If it succeeds, the project is fully configured 


### Chalenge #0

Inspect the solution and make sense of all the projects and the relationship between them.
Could you look over the project dependencies and determine where the API call lands initially and how it goes through the onion layers to the business logic and infra layer? 
Try to draw a diagram if you find it easier.

### Chalenge #1 

The app's end users just reported a bug: They cannot see the description after creating a to-do. 
Can you see what is going on?

### Challenge #2
The end users want to receive an email when they add a new todo. For the sake of simplicity, you can just call a webhook instead of sending emails. The service INotificationService is already in place, but something is wrong. Can you spot what it is? 

Hint: To use notifications, you can inspect the built-in types.

### Challenge #3
Your manager has decided that SQL is too expensive and would like to migrate the database to Azure Table Storage, which he believes is less expensive. You have been tasked with conducting an initial analysis and estimating the changes required for this migration. What classes or projects would be involved in this migration?

### Challenge #4
Deploy this architecture on an Azure stack and run it on Azure. What services from the ones created previously would you need?

1. Go to Azure Portal and grab the connection string of Azure SQL DB created in the last lab. If you don't have it, you can always redeploy the template

The connection string should look like this:
Server=tcp:[CHANGEMESERVER].database.windows.net,1433;Initial Catalog=arcacsqldb;Persist Security Info=False;User ID=andrei;Password=[CHANGEME];MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

2. In SQL Server also make sure firewall rules are open for your ip address
3. Go to appsettings.json and replace ToDoDatabase with the connection string above
4. Provision an AppService in Azure 
 Right-click publish from Visual Studio and either select your subscription or use the import publish profile option (profile can be downloaded from Azure - AppService overview page)

Enable Azure services access to the database from network configuration.

### Challenge #5 - Homework
The customer sends you a list of todos in a CSV file in the format name description.
How could you create a background process that will import the file regularly?

Hint: Which projects do you need to reference in the new Background worker process? What would be your element of reuse (could you leverage MediatR commands to process both Api and Command requests)?
