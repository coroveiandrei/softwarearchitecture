using MediatR;
using TaskManagement.Common;
using TaskManagement.Domain.Events;

namespace TaskManagement.Infrastructure.EventDispatcher
{
    [MapServiceDependency("EventDispatcher")]
    class EventDispatcher : IEventDispatcher
    {
        public IMediator _mediator;
        public EventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Publish(IEvent e)
        {
            _mediator.Publish(e);
        }
    }
}
