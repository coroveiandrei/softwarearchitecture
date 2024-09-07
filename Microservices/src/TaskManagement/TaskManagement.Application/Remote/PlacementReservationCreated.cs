
using MediatR;

namespace TaskManagement.Application.Remote
{
    public class PlacementReservationCreated(): IRequest
    {
        public string Location { get; set; }
        public string ObjectId { get; set; }
    }
}
