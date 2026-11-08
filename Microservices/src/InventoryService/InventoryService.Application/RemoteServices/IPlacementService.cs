using System.Threading.Tasks;
using System.Collections.Generic;

namespace InventoryService.Application.RemoteServices
{
    public interface IPlacementService
    {
        public Task<Dictionary<string, string>> GetReservedLocations();
        public Task<string[]> GetEligibleLocations(int take);
        public Task<bool> ReserveLocation(PlacementReservationModel placementModel);
        public Task<bool> ReleaseLocation(PlacementReservationModel placementModel);
    }
}
