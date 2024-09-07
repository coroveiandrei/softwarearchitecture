using InventoryService.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Services
{
    public interface IBaseEventPublisher
    {
        public Task PublishEvent(BaseEvent _event);
    }
}
