namespace PlacementService.Repositories;

public interface IPlacemementRepository
{
    Task<Dictionary<string, string>> GetReservedLocations();
    Task SavePlacementPosition(string location, string objectId);
    Task<string[]> GetEligiblePlacementPositions(int take);
}
