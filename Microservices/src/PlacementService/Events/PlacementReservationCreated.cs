namespace PlacementService.Events
{
    public record struct PlacementReservationCreated(Guid EventId, string ObjectId, string Location);
}
