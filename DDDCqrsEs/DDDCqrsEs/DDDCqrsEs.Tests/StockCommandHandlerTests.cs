using DDDCqrsEs.Application.Commands.StockCommands;
using DDDCqrsEs.Domain.Events;
using DDDCqrsEs.Domain.Models;
using DDDCqrsEs.Domain.Repositories;
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

        private sealed class CapturingEventStore : IEventStore
        {
            public BaseEvent SavedEvent { get; private set; }

            public Task SaveEvent(BaseEvent domainEvent, CancellationToken cancellationToken = default)
            {
                SavedEvent = domainEvent;
                return Task.CompletedTask;
            }

            public Task MarkEventPublished(Guid aggregateId, int version, CancellationToken cancellationToken = default) =>
                Task.CompletedTask;

            public IAsyncEnumerable<StoredEvent> GetEventsByAggregateId(Guid id, CancellationToken cancellationToken = default) =>
                Empty(cancellationToken);

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
