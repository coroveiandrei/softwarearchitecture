using DDDCqrsEs.Common;
using DDDCqrsEs.Domain.Events;
using DDDCqrsEs.Domain.Models;
using DDDCqrsEs.Domain.Repositories;
using DDDCqrsEs.Persistance.DataModel;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using DDDCqrsEs.Domain.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DDDCqrsEs.Persistance.Repositories
{
    [MapServiceDependency(Name: nameof(EventStore))]
    public class EventStore : IEventStore
    {
        private readonly ITableStorageConnection _connectionCreator;

        public EventStore(ITableStorageConnection connection) => _connectionCreator = connection;

        public async Task SaveEvent(BaseEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var entity = new EventEntity(domainEvent.AggregateId, domainEvent.Version)
            {
                EventType = domainEvent.EventType,
                Data = JsonConvert.SerializeObject(domainEvent.Stock),
                TimeCreated = domainEvent.TimeStamp,
                IsPublished = false
            };

            var table = await _connectionCreator.CreateConnection(nameof(EventStore));
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await table.ExecuteAsync(TableOperation.Insert(entity));
            }
            catch (StorageException exception) when (exception.RequestInformation?.HttpStatusCode == 409)
            {
                throw new OptimisticConcurrencyException(exception);
            }
        }

        public async IAsyncEnumerable<StoredEvent> GetEventsByAggregateId(Guid id,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString());
            var query = new TableQuery<EventEntity>().Where(filter);

            await foreach (var storedEvent in ExecuteQuery(query, cancellationToken))
            {
                yield return storedEvent;
            }
        }

        public async IAsyncEnumerable<StoredEvent> GetPendingEvents(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var filter = TableQuery.GenerateFilterConditionForBool(nameof(EventEntity.IsPublished), QueryComparisons.Equal, false);
            var query = new TableQuery<EventEntity>().Where(filter);

            await foreach (var storedEvent in ExecuteQuery(query, cancellationToken))
            {
                yield return storedEvent;
            }
        }

        public async Task MarkEventPublished(Guid aggregateId, int version, CancellationToken cancellationToken = default)
        {
            var table = await _connectionCreator.CreateConnection(nameof(EventStore));
            cancellationToken.ThrowIfCancellationRequested();
            var result = await table.ExecuteAsync(
                TableOperation.Retrieve<EventEntity>(aggregateId.ToString(), version.ToString("D10")));
            var entity = result.Result as EventEntity;
            if (entity == null)
            {
                result = await table.ExecuteAsync(
                    TableOperation.Retrieve<EventEntity>(aggregateId.ToString(), version.ToString()));
                entity = result.Result as EventEntity;
            }

            if (entity == null)
            {
                throw new InvalidOperationException($"Event {aggregateId}/{version} was not found.");
            }

            entity.IsPublished = true;
            cancellationToken.ThrowIfCancellationRequested();
            await table.ExecuteAsync(TableOperation.Replace(entity));
        }

        private async IAsyncEnumerable<StoredEvent> ExecuteQuery(TableQuery<EventEntity> query,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var table = await _connectionCreator.CreateConnection(nameof(EventStore));
            TableContinuationToken continuationToken = null;
            var storedEvents = new List<StoredEvent>();

            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                var segment = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = segment.ContinuationToken;
                foreach (var entity in segment.Results)
                {
                    storedEvents.Add(new StoredEvent(Guid.Parse(entity.PartitionKey), int.Parse(entity.RowKey),
                        entity.EventType, entity.Data, entity.TimeCreated));
                }
            }
            while (continuationToken != null);

            foreach (var storedEvent in storedEvents.OrderBy(item => item.Version))
            {
                yield return storedEvent;
            }
        }
    }
}
