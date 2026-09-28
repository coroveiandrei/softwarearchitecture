using DDDCqrsEs.Domain.Events;
using DDDCqrsEs.WebJob.Services;
using MediatR;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DDDCqrsEs.Tests
{
    public class Challenge2SubscriberTests
    {
        [Fact(Skip = "Challenge #2: remove Skip after dispatching received events through MediatR.")]
        public async Task Received_event_is_dispatched_to_its_projection_handler()
        {
            var mediator = Substitute.For<IMediator>();
            var handler = new ServiceBusQueueHandler("unused", "unused", mediator);
            var domainEvent = new StockDeleted(Guid.NewGuid(), 2, DateTime.UtcNow);

            await handler.DispatchEvent(domainEvent, CancellationToken.None);

            await mediator.Received(1).Send(domainEvent, Arg.Any<CancellationToken>());
        }
    }
}
