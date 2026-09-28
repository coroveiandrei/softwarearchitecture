# Clean Architecture Challenges

## Prepare and start the solution

1. Start Docker Desktop.
2. From `CleanArchitecture\infra`, run `start-all.ps1` to start the local SQL Server container.
3. Open `CleanArchitecture\CleanArc\CleanArc.sln` with the .NET 8 SDK installed.
4. Build and test the solution:

   ```powershell
   dotnet build
   dotnet test
   ```

5. Start the API:

   ```powershell
   dotnet run --project CleanArc.WebUI
   ```

6. Open `https://localhost:2001/swagger/index.html`.
7. Use the login endpoint with user `admin` and password `P@ssw0rd`.
8. Add a ToDo and list the user's ToDos to verify the environment.

## Challenge 0: Inspect the solution

Inspect the solution structure and the dependencies between its projects. Trace where the `AddToDo` API call first enters the application and how it reaches the business logic and infrastructure code.

## Challenge 1: Make the failing regression test pass

Users cannot see the description after creating a ToDo. The application test project contains a failing regression test that describes the expected behavior.

1. Run `dotnet test` and confirm that `AddToDo_PersistsDescription` fails.
2. Trace how the command model maps to the domain entity.
3. Fix the production code without changing the regression test.
4. Run the full test suite and confirm that all tests pass.

## Challenge 2: Move the outbound adapter to the correct layer

Users receive a webhook notification when they add a ToDo. The generic architecture test `ApplicationLayer_DoesNotImplementItsOutboundPorts` fails because the Application project contains a concrete implementation of one of its own outbound ports.

1. Run the architecture test and inspect the reported violating type.
2. Keep `INotificationService` in the Application project because it defines the port required by the use case.
3. Move the concrete HTTP implementation to the Infrastructure project.
4. Register the adapter in the composition root without adding an Application-to-Infrastructure dependency.
5. Run the full test suite and confirm that the architecture test passes.

As an extension, replace the directly constructed `HttpClient` with an injected client and move the webhook URL into configuration. Discuss what should happen when persistence succeeds but notification delivery fails.

## Challenge 3: Group architecture decision

Work in groups of three or four. A manager believes that Azure SQL is too expensive and proposes moving the ToDo data to Azure Table Storage.

Treat the proposal as a hypothesis. Prepare a short architecture decision record that covers:

- The data and access patterns that would move. State whether ASP.NET Identity data remains in SQL.
- A partition-key and row-key strategy for ToDos.
- Changes required in each project and any abstractions that currently assume Entity Framework or relational transactions.
- Your recommendation and a rough implementation estimate.

Each group has 20 minutes to prepare and 5 minutes to present. Other groups should challenge one assumption or trade-off in the proposal.
