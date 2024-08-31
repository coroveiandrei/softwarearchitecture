using PlacementService.Models;
using PlacementService.Repositories;

namespace PlacementService.Controllers;

[ApiController]
[Route("")]
public class PlacementController : ControllerBase
{
    private readonly ILogger<PlacementController> _logger;
    private readonly IPlacemementRepository _placementRepository;
    private readonly string _roadId;

    public PlacementController(
        ILogger<PlacementController> logger,
        IPlacemementRepository placementRepository)
    {
        _logger = logger;
        _placementRepository = placementRepository;
    }

    [HttpGet("GetEligibleLocations")]
    public async Task<string[]> GetEligibleLocations()
    {
        return await _placementRepository.GetEligiblePlacementPositions();
    }

    [HttpPost("ReserveLocation")]
    public async Task<ActionResult> ReserveLocation([FromBody] PlacementReservationModel placementReservation)
    {
        try
        {
            // should also add logic for Location state / Occupied / free / etc
            // log entry
            _logger.LogInformation($"ReservingLocation {placementReservation.Location} by {placementReservation.ObjectId}");
            await _placementRepository.SavePlacementPosition(placementReservation.Location, placementReservation.ObjectId);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing ENTRY");
            return StatusCode(500);
        }
    }

    [HttpPost("ReleaseLocation")]
    public async Task<ActionResult> ReleaseLocation([FromBody] PlacementReleaseModel placementReleaseModel)
    {
        try
        {
            // log entry
            _logger.LogInformation($"ReleasingLocation {placementReleaseModel.Location}");

            await _placementRepository.SavePlacementPosition(placementReleaseModel.Location, null);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing ENTRY");
            return StatusCode(500);
        }
    }
}
