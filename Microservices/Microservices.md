# Microservices

## Map and start the solution

### Prerequisites

- .NET 8 SDK
- Docker Desktop
- [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)

After installing the CLI, initialize the local Dapr runtime once:

```powershell
dapr init
dapr --version
```

`dapr init` starts the Redis state store and Zipkin tracing containers used by this sample.

### Start the applications

1. Clone the repository and navigate to the `Microservices` folder. Run the startup commands below from that folder.
2. Make sure Docker Desktop is running.
3. Start SQL Server, Azurite, and RabbitMQ:

   ```powershell
   ./start-microservices-infrastructure.ps1
   ```

   The scripts reuse an existing workshop container when one is already present.

4. In a new PowerShell window, start Task Management:

   ```powershell
   ./start-taskmanagement.ps1
   ```

   The application listens on `http://localhost:7000`.

5. In another PowerShell window, start Placement Service:

   ```powershell
   ./start-placement.ps1
   ```

   The application listens on `http://localhost:6000`.

6. In another PowerShell window, start Inventory Service:

   ```powershell
   ./start-inventory.ps1
   ```

   The application listens on `http://localhost:4000`.

7. Open `http://localhost:4000` and create a stock item. The item should receive a location, and Task Management should log the placement task.

The sample contains three distinct communication steps:

1. Inventory Service invokes Placement Service synchronously.
2. Placement Service stores the reservation using Dapr state management.
3. Placement Service publishes an event that Task Management receives asynchronously.

## Challenge #1 — Service invocation and retries

![Service invocation](https://github.com/user-attachments/assets/59c968dc-500f-4f5c-8825-328e6cca7e7d)

1. Trace how Inventory Service invokes Placement Service. Identify the application interface, its infrastructure implementation, and the Dapr sidecar call.
2. Inspect `src/dapr/components/resiliency.yaml`. Which target and HTTP status codes activate the service-invocation retry policy?
3. Stop Placement Service and restart it with two deterministic failures per stock configured:

   ```powershell
   $env:PLACEMENT_FAILURE_COUNT=2
   ./start-placement.ps1
   ```

4. Create a stock item. Use the Placement Service logs to determine how many calls were made before the reservation succeeded.
5. Restart Placement Service with `$env:PLACEMENT_FAILURE_COUNT=-1`, which makes every reservation attempt fail. How many total attempts occur, including the original request?
6. Explain why retrying `ReserveLocation` is safe or unsafe. What would have to change if the service failed after saving the reservation but before returning the response?

Remove the environment variable when finished:

```powershell
Remove-Item Env:PLACEMENT_FAILURE_COUNT
```

## Challenge #2 — Pub/sub, redelivery, and idempotency

![Pub/sub](https://github.com/user-attachments/assets/be798813-4fe6-450c-a4c0-4b42bef0da5c)

1. Trace how Placement Service publishes `PlacementReservationCreated` and how Task Management subscribes to it.
2. Restart Task Management with two deterministic failures configured:

   ```powershell
   $env:PLACEMENT_EVENT_FAILURE_COUNT=2
   ./start-taskmanagement.ps1
   ```

3. Create a stock item and use the event ID in the logs to verify that the same event is delivered again.
4. Restart Task Management with `$env:PLACEMENT_EVENT_FAILURE_COUNT=-1`. Follow the message through the configured retries until `ReceivePlacementDeadLetter` logs it. Use the RabbitMQ management UI at `http://localhost:15672` (`guest` / `guest`) to inspect the exchanges and queues involved.
5. Dapr pub/sub provides at-least-once delivery. Identify what would happen if the handler completed its business operation but failed before acknowledging the message.
6. Propose how Task Management could use `EventId` to make the handler idempotent. Which component should store the processed event IDs?

Remove the environment variable when finished:

```powershell
Remove-Item Env:PLACEMENT_EVENT_FAILURE_COUNT
```

## Challenge #3 — Consistency across services

Work in groups. You have **20 minutes to prepare** and **5 minutes to present**.

Consider this failure:

> Placement Service saves a location reservation, but publishing `PlacementReservationCreated` fails. The location is occupied while Task Management never receives the work item.

Prepare a design that keeps the workflow correct under publish failures, timeouts, and duplicate messages.

Focus on:

- what changes in `StockCommandHandler`, `PlacementController`, `PlacementRepository`, and `ToDoEventsController`;
- how a transactional outbox would ensure that saved reservations are eventually published;
- how a saga would coordinate the workflow and compensate a reservation when the workflow cannot complete;
- what the caller should observe while the workflow is incomplete.

Be ready to explain the trade-offs in your design rather than only naming infrastructure services.

Reference patterns for Challenge #3.

- [Transactional outbox](https://microservices.io/patterns/data/transactional-outbox.html) explains how to save a business change and an outgoing event atomically, then publish through a message relay.
- [Saga](https://microservices.io/patterns/data/saga.html) explains how to coordinate local transactions across services and compensate completed steps after a business failure.
