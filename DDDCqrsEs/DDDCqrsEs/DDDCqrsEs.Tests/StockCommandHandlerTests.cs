using DDDCqrsEs.Application.Commands.StockCommands;
using DDDCqrsEs.Domain.Events;
using DDDCqrsEs.Domain.Models;
using DDDCqrsEs.Domain.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DDDCqrsEs.Tests
{
    public class StockCommandHandlerTests
    {
        [Fact]
        public async Task Create_persists_the_new_event()
        {
            var eventStore = new CapturingEventStore();
            var handler = new StockCommandHandler(eventStore);

            var response = await handler.Handle(new CreateStockCommand { Item = "Coffee" }, CancellationToken.None);

            var savedEvent = Assert.IsType<StockCreated>(eventStore.SavedEvent);
            Assert.Equal(response.AggregateId, savedEvent.AggregateId);
            Assert.Equal(1, response.Version);
            Assert.Equal("Coffee", savedEvent.Stock.Item);
        }

        [Fact]
        public async Task Update_replays_history_and_persists_the_next_event()
        {
            var id = Guid.NewGuid();
            var eventStore = new CapturingEventStore(CreateHistory(id));
            var handler = new StockCommandHandler(eventStore);

            var response = await handler.Handle(new UpdateStockCommand
            {
                Id = id,
                Version = 1,
                Item = "Tea"
            }, CancellationToken.None);

            var savedEvent = Assert.IsType<StockUpdated>(eventStore.SavedEvent);
            Assert.Equal(2, response.Version);
            Assert.Equal("Tea", savedEvent.Stock.Item);
        }

        [Fact]
        public async Task Delete_replays_history_and_persists_the_next_event()
        {
            var id = Guid.NewGuid();
            var eventStore = new CapturingEventStore(CreateHistory(id));
            var handler = new StockCommandHandler(eventStore);

            var response = await handler.Handle(new DeleteStockCommand { Id = id, Version = 1 }, CancellationToken.None);

            Assert.IsType<StockDeleted>(eventStore.SavedEvent);
            Assert.Equal(2, response.Version);
        }

        private static StoredEvent CreateHistory(Guid id) => new StoredEvent(
            id,
            1,
            nameof(StockCreated),
            JsonConvert.SerializeObject(new StockModel { Item = "Coffee" }),
            DateTime.UtcNow);

        private sealed class CapturingEventStore : IEventStore
        {
            private readonly StoredEvent[] _history;

            public CapturingEventStore(params StoredEvent[] history) => _history = history;

            public BaseEvent SavedEvent { get; private set; }

            public Task SaveEvent(BaseEvent domainEvent, CancellationToken cancellationToken = default)
            {
                SavedEvent = domainEvent;
                return Task.CompletedTask;
            }

            public Task MarkEventPublished(Guid aggregateId, int version, CancellationToken cancellationToken = default) =>
                Task.CompletedTask;

            public async IAsyncEnumerable<StoredEvent> GetEventsByAggregateId(Guid id,
                [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                foreach (var storedEvent in _history)
                {
                    yield return storedEvent;
                }
                await Task.CompletedTask;
            }

            public IAsyncEnumerable<StoredEvent> GetPendingEvents(CancellationToken cancellationToken = default) =>
                Empty(cancellationToken);

            private static async IAsyncEnumerable<StoredEvent> Empty([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                await Task.CompletedTask;
                yield break;
            }
        }
    }
}
