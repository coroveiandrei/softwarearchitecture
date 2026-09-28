using System.Threading;
using System.Threading.Tasks;

namespace CleanArc.Application.Services
{
    public interface INotificationService
    {
        Task NotifyAsync(string message, CancellationToken cancellationToken = default);
    }
}
