namespace PlacementService.Repositories;

public interface IPlacemementRepository
{
    Task SavePlacementPosition(string location, string objectId);
    Task<string[]> GetEligiblePlacementPositions(int take);
}
