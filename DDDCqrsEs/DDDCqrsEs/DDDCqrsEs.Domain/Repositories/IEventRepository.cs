using DDDCqrsEs.Domain.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using DDDCqrsEs.Domain.Models;

namespace DDDCqrsEs.Domain.Repositories
{
    public interface IEventStore
    {
        IAsyncEnumerable<StoredEvent> GetEventsByAggregateId(Guid id, CancellationToken cancellationToken = default);
        Task SaveEvent(BaseEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
