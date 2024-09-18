using InventoryService.Application.RemoteServices;
using InventoryService.Common;
using Polly.Retry;
using Polly;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;

namespace InventoryService.Infrastructure.RemoteServices
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
            AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(response => (int)response.StatusCode >= 500 && (int)response.StatusCode <= 599)
            .RetryAsync(5, onRetry: (response, retryCount) =>
            {
                Console.WriteLine($"Retrying... Attempt #{retryCount} after receiving status code: {response.Result.StatusCode}");
            });

            var result = await retryPolicy.ExecuteAsync(async () =>
            {
                return await _httpClient.PostAsJsonAsync("ReserveLocation", placementModel);
            });
            
            return result.IsSuccessStatusCode;
        }

    }
}
