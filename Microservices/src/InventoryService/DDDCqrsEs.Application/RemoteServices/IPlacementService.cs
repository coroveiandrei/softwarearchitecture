using System.Threading.Tasks;

namespace DDDCqrsEs.Application.RemoteServices
{
    public interface IPlacementService
    {
        public Task<string[]> GetEligibleLocations(int take);
        public Task<bool> ReserveLocation(PlacementReservationModel placementModel);
        public Task<bool> ReleaseLocation(PlacementReservationModel placementModel);
    }
}
