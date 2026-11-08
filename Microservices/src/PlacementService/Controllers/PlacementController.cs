using PlacementService.Events;
using PlacementService.Models;
using PlacementService.Repositories;
using PlacementService.FailureSimulation;

namespace PlacementService.Controllers;

[ApiController]
[Route("")]
public class PlacementController : ControllerBase
{
    private readonly ILogger<PlacementController> _logger;
    private readonly IPlacemementRepository _placementRepository;
    private readonly ReservationFailureSimulator _failureSimulator;

    public PlacementController(
        ILogger<PlacementController> logger,
        IPlacemementRepository placementRepository,
        ReservationFailureSimulator failureSimulator)
    {
        _logger = logger;
        _placementRepository = placementRepository;
        _failureSimulator = failureSimulator;
    }

    [HttpGet("GetEligibleLocations")]
    public async Task<string[]> GetEligibleLocations([FromQuery] int? take)
    {
        _logger.LogInformation($"GetEligibleLocations {take}");
        return await _placementRepository.GetEligiblePlacementPositions(take ?? 10);
    }

    [HttpPost("ReserveLocation")]
    public async Task<ActionResult> ReserveLocation([FromBody] PlacementReservationModel placementReservation, [FromServices] DaprClient daprClient)
    {
        try
        {
            _logger.LogInformation($"ReservingLocation {placementReservation.Location} by {placementReservation.ObjectId}");
            if (_failureSimulator.ShouldFail())
            {
                _logger.LogWarning(
                    "Simulating a transient failure while reserving {Location} for {ObjectId}",
                    placementReservation.Location,
                    placementReservation.ObjectId);
                return StatusCode(503);
            }

            await _placementRepository.SavePlacementPosition(placementReservation.Location, placementReservation.ObjectId);
            await daprClient.PublishEventAsync(
                "pubsub",
                "placementEvents",
                new PlacementReservationCreated(Guid.NewGuid(), placementReservation.ObjectId, placementReservation.Location));


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
