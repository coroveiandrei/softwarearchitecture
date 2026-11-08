using PlacementService.Models;
using System;

namespace PlacementService.Repositories;

public class PlacementRepository : IPlacemementRepository
{
    private const string DAPR_STORE_NAME = "statestore";
    private readonly DaprClient _daprClient;
    private readonly string[] AllLocations;


    public PlacementRepository(DaprClient daprClient)
    {
        _daprClient = daprClient;
        AllLocations = InitilizeLocations();
    }

    private string[] InitilizeLocations()
    {
        string[] result = new string[260];
        int index = 0;

        for (char letter = 'A'; letter <= 'Z'; letter++)
        {
            // Loop through numbers 1 to 10
            for (int number = 1; number <= 10; number++)
            {
                // Combine letter and number and add to the array
                result[index++] = $"{letter}-{number}";
            }
        }
        return result;
    }

    public async Task<Dictionary<string, string>> GetReservedLocations()
    {
        var reservations = await _daprClient.GetBulkStateAsync<string>(
            DAPR_STORE_NAME, AllLocations, parallelism: 10);
        var locations = new Dictionary<string, string>();
        foreach (var reservation in reservations)
        {
            if (!string.IsNullOrEmpty(reservation.Value))
            {
                locations.TryAdd(reservation.Value, reservation.Key);
            }
        }
        return locations;
    }

    public async Task SavePlacementPosition(string location, string objectId)
    {
        await _daprClient.SaveStateAsync(
            DAPR_STORE_NAME, location, objectId);
    }

    public async Task<string[]> GetEligiblePlacementPositions(int take)
    {
        var emptyLocations = new List<string>();
        foreach (var location in AllLocations)
        {
            var result = await _daprClient.GetStateAsync<string>(DAPR_STORE_NAME, location);
            if (string.IsNullOrEmpty(result) && take > 0)
            {
                emptyLocations.Add(location);
                take--;
            }
        }
        return emptyLocations.ToArray();
    }
}
