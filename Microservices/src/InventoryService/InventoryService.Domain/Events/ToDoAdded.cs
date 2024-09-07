using System;

namespace InventoryService.Domain.Events
{
    public class ToDoAdded : IEvent
    {
        public Guid TodoId { get; set; }

        public ToDoAdded(Guid todoId)
        {
            TodoId = todoId;
        }
    }
}
