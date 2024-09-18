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
2. Simmulate a transient failure by stopping PlacementService and opening it up after you do the call to Inventory Service. Is the call processing retried? 
3. Simulate a long-term failure by stopping PlacementService. How many retries are made before the service abandons the call?

### Challenge #2 - DAPR Pub/Sub - AsyncCommunication
![image](https://github.com/user-attachments/assets/be798813-4fe6-450c-a4c0-4b42bef0da5c)

1. Inspect how PlacementService calls TaskManagement
2. Simmulate a transient failure by stopping TaskManagement and opening it up after the initial call. Is the message processed? 
3. Simulate a long-term failure by  stopping TaskManagement. Start TaskManagement after a few minutes. Is the message processed?

 ### Challenge #3 - Working in groups come up with a retry strategy for sync communication
 
 Use this questions to get you started:
 1. Which HTTP result codes would you retry on? Which not?
 2. What are the timeouts after which you would retry? What kind of retry strategy would you advise using?
 3. Think about a high-throughput environment with thousands of calls per second where the environment all of a sudden gives 500. How can you align the timeouts of the service so that after recovery not all calls are done at the same time? Can you bring up a strategy for this?
    
