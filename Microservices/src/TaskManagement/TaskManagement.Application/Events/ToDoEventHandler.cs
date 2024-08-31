using MediatR;
using TaskManagement.Domain.Events;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Services;

namespace TaskManagement.Application.Events
{
    public class ToDoEventHandler : INotificationHandler<ToDoAdded>
    {
        private readonly INotificationService _notificationService;
        public ToDoEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public ToDoEventHandler()
        {
            
        }

        public async Task Handle(ToDoAdded todoAdded, CancellationToken cancellationToken)
        {
            // handle the event here
        }
    }
}
