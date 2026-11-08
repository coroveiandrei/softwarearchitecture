using InventoryService.Application.RemoteServices;
using InventoryService.Common;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.RemoteServices
{
    public class PlacementService : IPlacementService
    {
        private HttpClient _httpClient;
        public PlacementService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<Dictionary<string, string>> GetReservedLocations()
        {
            return _httpClient.GetFromJsonAsync<Dictionary<string, string>>("GetReservedLocations");
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
            using var content = JsonContent.Create(placementModel);
            await content.LoadIntoBufferAsync();
            using var result = await _httpClient.PostAsync("ReserveLocation", content);
            return result.IsSuccessStatusCode;
        }

    }
}
