using System.Threading.Tasks;
using DDDCqrsEs.Common;
using DDDCqrsEs.Domain.Events;
using MediatR;

namespace DDDCqrsEs.Application.Services
{
    [MapServiceDependency(Name: nameof(EventPublisher))]
    public class EventPublisher : IBaseEventPublisher
    {
        private IMediator _mediator;

        public EventPublisher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task PublishEvent(BaseEvent _event)
        {
            await _mediator.Send(_event);
        }
    }
}
