using System;

namespace TaskManagement.Domain.Events
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
