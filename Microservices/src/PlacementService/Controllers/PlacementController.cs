using PlacementService.Events;
using PlacementService.Models;
using PlacementService.Repositories;

namespace PlacementService.Controllers;

[ApiController]
[Route("")]
public class PlacementController : ControllerBase
{
    private readonly ILogger<PlacementController> _logger;
    private readonly IPlacemementRepository _placementRepository;

    public PlacementController(
        ILogger<PlacementController> logger,
        IPlacemementRepository placementRepository)
    {
        _logger = logger;
        _placementRepository = placementRepository;
    }

    [HttpGet("GetEligibleLocations")]
    public async Task<string[]> GetEligibleLocations([FromQuery] int take)
    {
        _logger.LogInformation($"GetEligibleLocations {take}");
        return await _placementRepository.GetEligiblePlacementPositions(take);
    }

    [HttpPost("ReserveLocation")]
    public async Task<ActionResult> ReserveLocation([FromBody] PlacementReservationModel placementReservation, [FromServices] DaprClient daprClient)
    {
        try
        {
            _logger.LogInformation($"ReservingLocation {placementReservation.Location} by {placementReservation.ObjectId}");
            await _placementRepository.SavePlacementPosition(placementReservation.Location, placementReservation.ObjectId);
            await daprClient.PublishEventAsync("pubsub", "placementEvents", new PlacementReservationCreated(placementReservation.ObjectId, placementReservation.Location));

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
