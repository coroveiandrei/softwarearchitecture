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
            _logger.LogInformation("Reservation attempt received | Stock={ObjectId} Location={Location}", placementReservation.ObjectId, placementReservation.Location);
            if (_failureSimulator.ShouldFail())
            {
                _logger.LogWarning(
                    "Reservation rejected with HTTP 503 by failure simulator | Location={Location} Stock={ObjectId}",
                    placementReservation.Location,
                    placementReservation.ObjectId);
                return StatusCode(503);
            }

            await _placementRepository.SavePlacementPosition(placementReservation.Location, placementReservation.ObjectId);
            _logger.LogInformation("Reservation saved | Stock={ObjectId} Location={Location}", placementReservation.ObjectId, placementReservation.Location);
            var placementEvent = new PlacementReservationCreated(Guid.NewGuid(), placementReservation.ObjectId, placementReservation.Location);
            await daprClient.PublishEventAsync(
                "pubsub",
                "placementEvents",
                placementEvent);
            _logger.LogInformation("Placement event published; returning HTTP 200 | Event={EventId} Stock={ObjectId} Location={Location}", placementEvent.EventId, placementEvent.ObjectId, placementEvent.Location);


            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Placement operation failed with HTTP 500");
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
            _logger.LogError(ex, "Placement operation failed with HTTP 500");
            return StatusCode(500);
        }
    }
}
