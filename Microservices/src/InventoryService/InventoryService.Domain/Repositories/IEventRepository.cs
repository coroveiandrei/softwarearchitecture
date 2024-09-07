using InventoryService.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;
using InventoryService.Application.Repositories.Base;
using System.Threading.Tasks;
using InventoryService.Persistance.DataModel;

namespace InventoryService.Domain.Repositories
{
    public interface IEventStore
    {
        public IAsyncEnumerable<EventEntity> GetEventsByAggregateId(Guid id);
        public IAsyncEnumerable<EventEntity> GetAllEvents();
        public Task SaveEvent(BaseEvent _event);
    }
}
