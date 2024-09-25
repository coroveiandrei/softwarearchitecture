# Microservices

### Map & start the solution:
1. Clone the repo. Navigate to Microservices folder.
2. Make sure you have DAPR, Docker Desktop, and .NET 8 SDK installed
DAPR Cli - https://docs.dapr.io/getting-started/install-dapr-cli/
Docker Desktop - https://www.docker.com/products/docker-desktop/
4. Make sure Docker Desktop is started
5. Start the infrastructure containers (localdb, azurite, rabbit). Go to /src/infrastructure and run **start-all.ps1**. This will spin up 3 docker containers:
- localdb - used for storage in TaskManagement and InventoryService
- azureite - used as table storage in InventoryService
- rabbit - used as broker for messaging mechanism

5. Start TaskManagement Project. 

Navigate to TaskManagement\TaskManagement.WebUI and run start-selfhosted.ps1. If all goes well, you should see:

```
== APP == info: Microsoft.Hosting.Lifetime[14]
== APP == Now listening on: http://localhost:7000
== APP == info: Microsoft.Hosting.Lifetime[0]
== APP == Application started. Press Ctrl+C to shut down.
```

Notice that it is a copy of the exercise we did for CleanArchitecture; it does not contain authentication.

6. Start PlacementService Project. 
Open another Powershell window. Navigate to PlacementService and run start-selfhosted.ps1. If all goes well, you should see:

```
== APP == info: Microsoft.Hosting.Lifetime[14]
== APP ==       Now listening on: http://localhost:6000
== APP == info: Microsoft.Hosting.Lifetime[0]
== APP ==       Application started. Press Ctrl+C to shut down.
```

7. Start InventoryService Project
Open another Powershell window. Navigate to InventoryService\InventoryService.WebUI and run start-selfhosted.ps1. If all goes well, you should see:

```
== APP == info: Microsoft.Hosting.Lifetime[14]
== APP ==       Now listening on: http://localhost:4000
== APP == info: Microsoft.Hosting.Lifetime[0]
== APP ==       Application started. Press Ctrl+C to shut down.
```

8. Test:

Open up http://localhost:4000 and create a stock. You should see that a location is received for the stock and also that in TaskManagement console a task has been made.

### Challenge #1 - Inspect Service Invocation - Sync Communication
![image](https://github.com/user-attachments/assets/59c968dc-500f-4f5c-8825-328e6cca7e7d)

1. Inspect how InventoryService calls PlacementService
2. Simulate a transient failure by uncommenting the region TransientIssue in PlacementService. Is the call processing retried? 
3. Simulate a long-term failure by uncommenting the region related to LongTimeFailure in PlacementService. How many retries are made before the service abandons the call?

### Challenge #2 - DAPR Pub/Sub - AsyncCommunication
![image](https://github.com/user-attachments/assets/be798813-4fe6-450c-a4c0-4b42bef0da5c)

1. Inspect how PlacementService calls TaskManagement
2. Simulate a transient failure by uncommenting the region TransientIssue in TaskManagement. Is the message reprocessed? 
3. Simulate a long-term failure by uncommenting the region related to LongTimeFailure in TaskManagement. How many retries are made before the message is abandoned?

### Challenge #3.a - Deployment strategy

You are tasked with sketching a deployment strategy for the system above.
1. What services would you use for hosting? Why?
2. What pipeline technology would you use? How would you configure that? Why?
3. Other things that you consider important


### Challenge #3.b - Testing strategy

 You are tasked with sketching a testing strategy for the system above. 
 Use this questions to get you started:
 1. What kind of testing would you use? What use cases do you think you will cover with each of the testig strategies?
 2. Think about security testing
 3. Think about performance testing. How would you conduct this?
 4. Other things that you consider important

  
 
 
