using MediatR; 
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Remote;
using Microsoft.Extensions.Logging;

namespace TaskManagement.Application.Events
{
    public class PlacementEventHandler : IRequestHandler<PlacementReservationCreated>
    {
        private readonly ILogger<PlacementEventHandler> _logger;

        public PlacementEventHandler(ILogger<PlacementEventHandler> logger)
        {
            _logger = logger;    
        }

        public async Task Handle(PlacementReservationCreated placementReservationCreated, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Placement created event received for {placementReservationCreated.ObjectId} on {placementReservationCreated.Location}. Please move your beautiful feet on the docks and move the stock to the appropriate location.");
        }
    }
}
