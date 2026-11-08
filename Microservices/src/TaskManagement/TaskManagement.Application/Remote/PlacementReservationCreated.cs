
using MediatR;
using System;

namespace TaskManagement.Application.Remote
{
    public class PlacementReservationCreated(): IRequest
    {
        public Guid EventId { get; set; }
        public string Location { get; set; }
        public string ObjectId { get; set; }
    }
}
