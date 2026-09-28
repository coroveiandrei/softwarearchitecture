using System.Threading;
using System.Threading.Tasks;

namespace DDDCqrsEs.Application.Services
{
    public interface IEventOutboxDispatcher
    {
        Task DispatchPendingEvents(CancellationToken cancellationToken = default);
    }
}
