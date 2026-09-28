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

## Challenge 1: Protect a bug fix with a regression test

Users previously lost the description when they created a ToDo. The application test project contains a regression test for this behavior.

1. Run `dotnet test` and inspect `ToDoCommandHandlerTests`.
2. Trace how the command model maps to the domain entity.
3. Explain why the mapping belongs in the application use case.
4. Add one more test for a related mapping or behavior that should not regress.

## Challenge 2: Review an outbound adapter

Users receive a notification when they add a ToDo. Review how the application reaches the webhook through `INotificationService`.

Verify that:

- The Application project owns the notification interface.
- The Infrastructure project owns the HTTP implementation.
- The composition root supplies the webhook URL and registers the adapter.
- The application use case can be tested without making an HTTP call.

Discuss what should happen when persistence succeeds but notification delivery fails. Document the policy your team recommends. As an extension, use the existing `ToDoAdded` event to separate notification delivery from the command handler.

## Challenge 3: Group architecture decision

Work in groups of three or four. A manager believes that Azure SQL is too expensive and proposes moving the ToDo data to Azure Table Storage.

Treat the proposal as a hypothesis. Prepare a short architecture decision record that covers:

- The data and access patterns that would move. State whether ASP.NET Identity data remains in SQL.
- A partition-key and row-key strategy for ToDos.
- Changes required in each project and any abstractions that currently assume Entity Framework or relational transactions.
- Differences in queries, consistency, transactions, migration, rollback, security, and operations.
- The evidence needed to compare cost and performance.
- Your recommendation and a rough implementation estimate.

Each group has 30 minutes to prepare and 5 minutes to present. Other groups should challenge one assumption or trade-off in the proposal.
