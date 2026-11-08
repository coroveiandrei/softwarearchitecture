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
   - Start the API in Development on a port separate from the Microservices sample:

   ```powershell
   $env:ASPNETCORE_ENVIRONMENT = "Development"
   dotnet run --no-launch-profile --urls http://localhost:5187
   ```

8. Navigate to `http://localhost:5187/swagger/index.html`. The stock endpoints allow anonymous requests. The documented `admin` account is available only if the database was seeded previously.
9. Run the starter tests from the solution folder:

   ```powershell
   dotnet test DDDCqrsEs.Tests
   ```

   The starter has three passing tests and four skipped Challenge #2 tests. Enable those skipped tests when implementing Challenge #2.

### Challenge #0: Project structure and dependencies

Inspect the overall project structure:

- What projects compose the solution?
- What are the links between them?
- Can you spot the resemblance to Clean Architecture?

Inspect the overall sequence diagram and determine why the solution has two entry-point projects: WebUI and WebJob.

![image](https://github.com/coroveiandrei/trainingsoftarchforcloud/assets/37452422/f502d235-16cf-48af-8917-fc170f909227)

### Challenge #1: Write path

In the first challenge, create the writing path. Create, update, and delete commands should each generate and persist one new domain event in Azure Table Storage. Read requests retrieve data without creating events.

Some code is already in place. Locate and complete it.

Hint: Create a table called `EventStore` in Table Storage to serve as the persistence mechanism.

The challenge is complete when `StockCreated`, `StockUpdated`, and `StockDeleted` are visible in `EventStore` for the same aggregate, with increasing versions. Use the aggregate ID returned by Create in subsequent Update and Delete requests. SQL projections start updating after Challenge #2 is completed.

### Challenge #2: Read path

In Challenge #2, read data from projections to optimize reads. Complete it in two stages.

First, publish each newly persisted aggregate event to Service Bus. Publish only the new event—not the historical events used to reconstitute the aggregate. Verify that the message reaches the queue.

Next, complete the `DDDCqrsEs.WebJob` subscriber so that the received event is dispatched to the existing projection handlers and updates SQL.

Hint: Create a queue named `StockEvents` in the Service Bus namespace.

Run the subscriber from `DDDCqrsEs.WebJob` in a separate terminal with the Service Bus and SQL environment variables from setup. Set its queue name to the same queue used by the API publisher:

```powershell
$env:ConnectionStrings__QueueName = "stockevents"
dotnet run
```

The challenge is complete when create, update, and delete events update the SQL projection. Enable the four skipped Challenge #2 publishing/subscriber tests and confirm that they pass.

After it works, consider this failure: Table Storage accepts the event, but Service Bus is temporarily unavailable. The command fails after the write model has already changed, and the read model may never receive the event. What consistency problem does this create, and how could the design guarantee that the event is eventually published without undoing the persisted write?

### Challenge #3: Versioning and concurrency

CQRS uses separate write and read models, so synchronization takes time. The application must handle potential lost updates.

For example, Alice and Bob may both have version 1 of a stock item and update it at the same time. One update should succeed while the other receives "Data was changed by another user."

![image](https://github.com/coroveiandrei/trainingsoftarchforcloud/assets/37452422/4f353a94-3213-4237-b8f1-854eba9da467)

Event version assignment and duplicate-key rejection are already implemented. Complete the concurrency behavior:

1. Compare the version supplied by Update or Delete with the latest persisted aggregate version. SQL projections can lag behind the event stream, so comparing only against the projection cannot guarantee that a command is current.
2. Retain the Event Store's insert-only behavior. If two commands read the same version concurrently, their new events use the same aggregate ID and version. Only one insert can succeed.
3. Return "Data was changed by another user" for a stale expected version or a competing insert that loses the race.

Verify this with two requests using the same expected version. One should succeed and the other should receive the conflict response.
