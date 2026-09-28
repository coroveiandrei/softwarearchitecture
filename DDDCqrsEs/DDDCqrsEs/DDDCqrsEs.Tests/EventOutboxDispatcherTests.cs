using DDDCqrsEs.Application.Services;
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
    public class EventOutboxDispatcherTests
    {
        [Fact]
        public async Task Marks_an_event_published_only_after_a_successful_send()
        {
            var storedEvent = CreateStoredEvent();
            var store = new FakeEventStore(storedEvent);
            var dispatcher = new EventOutboxDispatcher(store, new FakePublisher());

            await dispatcher.DispatchPendingEvents();

            Assert.Equal((storedEvent.AggregateId, storedEvent.Version), store.MarkedPublished);
        }

        [Fact]
        public async Task Leaves_an_event_pending_when_publishing_fails()
        {
            var store = new FakeEventStore(CreateStoredEvent());
            var dispatcher = new EventOutboxDispatcher(store, new FakePublisher(fail: true));

            await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.DispatchPendingEvents());

            Assert.Null(store.MarkedPublished);
        }

        private static StoredEvent CreateStoredEvent() => new StoredEvent(
            Guid.NewGuid(), 3, nameof(StockUpdated), JsonConvert.SerializeObject(new StockModel { Item = "Tea" }), DateTime.UtcNow);

        private sealed class FakePublisher : IBaseEventPublisher
        {
            private readonly bool _fail;
            public FakePublisher(bool fail = false) => _fail = fail;

            public Task PublishEvent(BaseEvent domainEvent, CancellationToken cancellationToken = default)
            {
                if (_fail) throw new InvalidOperationException("Service Bus unavailable");
                return Task.CompletedTask;
            }
        }

        private sealed class FakeEventStore : IEventStore
        {
            private readonly StoredEvent _pendingEvent;
            public FakeEventStore(StoredEvent pendingEvent) => _pendingEvent = pendingEvent;
            public (Guid AggregateId, int Version)? MarkedPublished { get; private set; }

            public async IAsyncEnumerable<StoredEvent> GetPendingEvents([EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                yield return _pendingEvent;
                await Task.CompletedTask;
            }

            public IAsyncEnumerable<StoredEvent> GetEventsByAggregateId(Guid id, CancellationToken cancellationToken = default) =>
                Empty(cancellationToken);

            public Task SaveEvent(BaseEvent domainEvent, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task MarkEventPublished(Guid aggregateId, int version, CancellationToken cancellationToken = default)
            {
                MarkedPublished = (aggregateId, version);
                return Task.CompletedTask;
            }

            private static async IAsyncEnumerable<StoredEvent> Empty([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                await Task.CompletedTask;
                yield break;
            }
        }
    }
}
