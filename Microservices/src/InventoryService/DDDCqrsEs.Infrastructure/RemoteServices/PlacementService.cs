using DDDCqrsEs.Application.RemoteServices;
using DDDCqrsEs.Common;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DDDCqrsEs.Infrastructure.RemoteServices
{
    public class PlacementService : IPlacementService
    {
        private HttpClient _httpClient;
        public PlacementService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string[]> GetEligibleLocations(int take)
        {
            return await _httpClient.GetFromJsonAsync<string[]>($"GetEligibleLocations?take={take}");
        }

        public async Task<bool> ReleaseLocation(PlacementReservationModel placementModel)
        {
            await _httpClient.PostAsJsonAsync("ReleaseLocation", placementModel);
            return true;
        }

        public async Task<bool> ReserveLocation(PlacementReservationModel placementModel)
        {
            await _httpClient.PostAsJsonAsync("ReserveLocation", placementModel);
            return true;
        }
    }
}
