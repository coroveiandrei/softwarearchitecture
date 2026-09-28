# DDD CQRS Event Sourcing Architecture Challenges

### Map and start the solution

1. From `infracode`, create a local parameter file that Git will ignore:

   ```powershell
   Copy-Item .\config\DDDInfrastructureparams.example.json .\config\DDDInfrastructureparams.json
   ```

2. Replace the `CHANGE_ME` values in the local parameter file, then deploy the resources:

   ```powershell
   az deployment group create --resource-group [CHANGEME] --template-file .\DDDInfrastructure.bicep --parameters .\config\DDDInfrastructureparams.json
   ```

3. Create a least-privilege Service Bus shared-access policy for the exercise. Do not use `RootManageSharedAccessKey` in application configuration.
4. Set the required connection strings as environment variables in each terminal used to start the Web UI or WebJob:

   ```powershell
   $env:ConnectionStrings__TableConnectionString = "[TABLE_STORAGE_CONNECTION_STRING]"
   $env:ConnectionStrings__ServiceBusString = "[SERVICE_BUS_CONNECTION_STRING]"
   $env:ConnectionStrings__ToDoDatabase = "[SQL_CONNECTION_STRING]"
   ```

   Keep real values out of `appsettings.json`, screenshots, and commits.

5. Add your IP address to the Azure SQL firewall rules.
6. Open the `DDDCqrsEs` solution and run `dotnet build`.
7. In the current terminal:
   - Make sure you are in the `DDDCqrsEs.WebUI` folder.
   - Run `dotnet run`.
8. Navigate to `https://localhost:[port]/swagger`.

Login with:

- User: `admin`
- Password: `P@ssw0rd`

### Challenge #0: Project structure and dependencies

Inspect the overall project structure:

- What projects compose the solution?
- What are the links between them?
- Can you spot the resemblance to Clean Architecture?

Inspect the overall sequence diagram and determine why the solution has two entry-point projects: WebUI and WebJob.

![image](https://github.com/coroveiandrei/trainingsoftarchforcloud/assets/37452422/f502d235-16cf-48af-8917-fc170f909227)

### Challenge #1: Write path

In the first challenge, create the writing path. A new event should be generated and stored in the Event Store in Table Storage whenever there is a web request.

Some code is already in place. Locate and complete it.

Hint: Create a table called `EventStore` in Table Storage to serve as the persistence mechanism.

The challenge is complete when the events `StockCreated`, `StockUpdated`, and `StockDeleted` are visible in `EventStore`.

### Challenge #2: Read path

In Challenge #2, read data from projections to optimize reads.

Publish events related to the aggregate to the messaging system. The `DDDCqrsEs.WebJob` process should subscribe to Service Bus messages and update the projections. Complete the existing publisher and subscriber code. First verify that messages reach the queue, then complete the subscriber.

Hint: Create a queue named `StockEvents` in the Service Bus namespace.

The challenge is complete when the stock appears in SQL.

### Challenge #3: Versioning and concurrency

CQRS uses separate write and read models, so synchronization takes time. The application must handle potential lost updates.

For example, Alice and Bob may both have version 1 of a stock item and update it at the same time. One update should succeed while the other receives "Data was changed by another user."

![image](https://github.com/coroveiandrei/trainingsoftarchforcloud/assets/37452422/4f353a94-3213-4237-b8f1-854eba9da467)

The solution checks this in two places:

1. The command validator checks whether the projection version matches the latest aggregate version.
2. The Event Store rejects duplicate keys. Using the version as the row key and aggregate ID as the partition key provides the concurrency check.

Versioning is already in place. Implement the "data was changed by another user" validation.
