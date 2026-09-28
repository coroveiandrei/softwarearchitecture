using DDDCqrsEs.Application.Commands.StockCommands;
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
    public class Challenge2PublishingTests
    {
        private const string SkipReason = "Challenge #2: remove Skip after wiring event publishing.";

        [Fact(Skip = SkipReason)]
        public async Task Create_publishes_the_new_event()
        {
            var operations = new List<string>();
            var publisher = new RecordingPublisher(operations);
            var handler = new StockCommandHandler(new FakeEventStore(operations), publisher);

            await handler.Handle(new CreateStockCommand { Item = "Coffee" }, CancellationToken.None);

            Assert.IsType<StockCreated>(Assert.Single(publisher.PublishedEvents));
            Assert.Equal(new[] { "save", "publish" }, operations);
        }

        [Fact(Skip = SkipReason)]
        public async Task Update_publishes_only_the_new_event()
        {
            var aggregateId = Guid.NewGuid();
            var operations = new List<string>();
            var publisher = new RecordingPublisher(operations);
            var handler = new StockCommandHandler(new FakeEventStore(operations, CreateHistory(aggregateId)), publisher);

            await handler.Handle(new UpdateStockCommand { Id = aggregateId, Item = "Tea" }, CancellationToken.None);

            Assert.IsType<StockUpdated>(Assert.Single(publisher.PublishedEvents));
            Assert.Equal(new[] { "save", "publish" }, operations);
        }

        [Fact(Skip = SkipReason)]
        public async Task Delete_publishes_only_the_new_event()
        {
            var aggregateId = Guid.NewGuid();
            var operations = new List<string>();
            var publisher = new RecordingPublisher(operations);
            var handler = new StockCommandHandler(new FakeEventStore(operations, CreateHistory(aggregateId)), publisher);

            await handler.Handle(new DeleteStockCommand { Id = aggregateId }, CancellationToken.None);

            Assert.IsType<StockDeleted>(Assert.Single(publisher.PublishedEvents));
            Assert.Equal(new[] { "save", "publish" }, operations);
        }

        private static StoredEvent CreateHistory(Guid aggregateId) => new StoredEvent(
            aggregateId,
            1,
            nameof(StockCreated),
            JsonConvert.SerializeObject(new StockModel { Item = "Coffee" }),
            DateTime.UtcNow);

        private sealed class RecordingPublisher : IBaseEventPublisher
        {
            private readonly List<string> _operations;
            public RecordingPublisher(List<string> operations) => _operations = operations;
            public List<BaseEvent> PublishedEvents { get; } = new List<BaseEvent>();

            public Task PublishEvent(BaseEvent domainEvent, CancellationToken cancellationToken = default)
            {
                PublishedEvents.Add(domainEvent);
                _operations.Add("publish");
                return Task.CompletedTask;
            }
        }

        private sealed class FakeEventStore : IEventStore
        {
            private readonly StoredEvent[] _history;
            private readonly List<string> _operations;
            public FakeEventStore(List<string> operations, params StoredEvent[] history)
            {
                _operations = operations;
                _history = history;
            }

            public async IAsyncEnumerable<StoredEvent> GetEventsByAggregateId(Guid id,
                [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                foreach (var storedEvent in _history)
                {
                    yield return storedEvent;
                }
                await Task.CompletedTask;
            }

            public Task SaveEvent(BaseEvent domainEvent, CancellationToken cancellationToken = default)
            {
                _operations.Add("save");
                return Task.CompletedTask;
            }
        }
    }
}
