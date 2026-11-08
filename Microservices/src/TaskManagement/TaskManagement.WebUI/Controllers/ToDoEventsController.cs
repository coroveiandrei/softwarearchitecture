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
                "Received placement event {EventId} for location {Location}",
                placementCreatedEvent.EventId,
                placementCreatedEvent.Location);

            if (_failureSimulator.ShouldFail())
            {
                _logger.LogWarning(
                    "Simulating a failure while processing placement event {EventId}",
                    placementCreatedEvent.EventId);
                return StatusCode(503);
            }

            await Mediator.Send(placementCreatedEvent);
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
