using DDDCqrsEs.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DDDCqrsEs.Application.Services
{
    public interface IBaseEventPublisher
    {
        Task PublishEvent(BaseEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
