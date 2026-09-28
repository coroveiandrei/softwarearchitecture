using DDDCqrsEs.Application.Common;
using DDDCqrsEs.Common;
using DDDCqrsEs.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace DDDCqrsEs.Application.Services
{
    [MapServiceDependency(Name: nameof(EventOutboxDispatcher))]
    public class EventOutboxDispatcher : IEventOutboxDispatcher
    {
        private readonly IEventStore _eventStore;
        private readonly IBaseEventPublisher _publisher;

        public EventOutboxDispatcher(IEventStore eventStore, IBaseEventPublisher publisher)
        {
            _eventStore = eventStore;
            _publisher = publisher;
        }

        public async Task DispatchPendingEvents(CancellationToken cancellationToken = default)
        {
            await foreach (var storedEvent in _eventStore.GetPendingEvents(cancellationToken))
            {
                var domainEvent = EventMapper.ConvertFromStoredEvent(storedEvent);
                await _publisher.PublishEvent(domainEvent, cancellationToken);
                await _eventStore.MarkEventPublished(storedEvent.AggregateId, storedEvent.Version, cancellationToken);
            }
        }
    }
}
