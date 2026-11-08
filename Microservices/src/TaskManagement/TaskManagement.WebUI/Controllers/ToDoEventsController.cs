using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.WebUI.Controllers.Base;
using System.Threading.Tasks;
using Dapr;
using TaskManagement.Application.Remote;
using Microsoft.Extensions.Logging;
using System;
using TaskManagement.WebUI.FailureSimulation;

namespace TaskManagement.WebUI.Controllers
{
    [ApiController]
    [Route("")]
    public class ToDoEventsController : BaseController
    {
        private readonly ILogger<ToDoEventsController> _logger;
        private readonly PlacementEventFailureSimulator _failureSimulator;
        
        public ToDoEventsController(
            ILogger<ToDoEventsController> logger,
            PlacementEventFailureSimulator failureSimulator) {
            _logger = logger;
            _failureSimulator = failureSimulator;
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Topic("pubsub", "placementEvents", "deadletters", false)]
        public async Task<ActionResult> ReceivePlacementEvent([FromBody] PlacementReservationCreated placementCreatedEvent)
        {
            _logger.LogInformation(
                "Placement event delivery received | Event={EventId} Location={Location}",
                placementCreatedEvent.EventId,
                placementCreatedEvent.Location);

            if (_failureSimulator.ShouldFail())
            {
                _logger.LogWarning(
                    "Event delivery rejected with HTTP 503 by failure simulator; eligible for redelivery | Event={EventId}",
                    placementCreatedEvent.EventId);
                return StatusCode(503);
            }

            await Mediator.Send(placementCreatedEvent);
            _logger.LogInformation("Placement event processed; returning HTTP 200 | Event={EventId} Stock={ObjectId}", placementCreatedEvent.EventId, placementCreatedEvent.ObjectId);
            return Ok();
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Topic("pubsub", "deadletters")]
        public ActionResult ReceivePlacementDeadLetter([FromBody] PlacementReservationCreated placementCreatedEvent)
        {
            _logger.LogError(
                "Placement event {EventId} for {ObjectId} was moved to the dead-letter topic",
                placementCreatedEvent.EventId,
                placementCreatedEvent.ObjectId);

            return Ok();
        }
    }
   
}
