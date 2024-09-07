# Microservices

### Map & start the solution:
1. Clone the repo. Navigate to Microservices folder.
2. Make sure you have DAPR, Docker Desktop, and .NET 8 SDK installed
DAPR Cli - https://docs.dapr.io/getting-started/install-dapr-cli/
Docker Desktop - https://www.docker.com/products/docker-desktop/
3. In a cmd run:  _dapr init_
4. Start the infrastructure containers (localdb, azurite, rabbit). Go to /src/infrastructure and run start-all.ps1. This will spin up 3 docker containers:
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

Open up http://localhost:4000 and create a stock. You should see that a location is received for the stock and also that in TaskManagement console a task has been created.

### Challenge #1 - Inspect State Store

1. Open up TrafficControllerService project and inspect DAPRVechicleStateRepository
* a DAPRClient is injected that has access to statestore
* via DAPRClient we access and save the state of the vehicle

2. Inspect the underlying store. TrafficControl uses something to store the state
* Navigate to src/dapr/components
* could you identify what software component  it uses to store state?


### Challenge #2 - Inspect Service Invocation

Inspect the Invoke between FineCollectionService and VehicleRegistrationService
1. In CollectionController a call is made to proxy vechicleRegistrationService.GetVehicleInfo
2. Get Vechicle info calls the Vechicle microservice using HTTP Client. It also uses the auto deserialization from JSON
3. Inspect how VehicleRegistrationService is registered in the container. Is it singleton, or is it transient? Why? 
4. Inspect the Program.cs how the HTTPClient is created


### Challenge #3 - DAPR Pub/Sub
1. For the publisher part, inspect TrafficController.VehicleExitAsync, the last line of code of the method.
2. For the subscriber part, inspect CollectionController.CollectFine
3. For the configuration part, navigate to /dapr/components/pubsub.yaml
 
