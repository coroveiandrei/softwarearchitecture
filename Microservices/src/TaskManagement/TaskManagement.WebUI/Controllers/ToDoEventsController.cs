using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.WebUI.Controllers.Base;
using System.Threading.Tasks;
using Dapr;
using TaskManagement.Application.Remote;
using Microsoft.Extensions.Logging;
using System;

namespace TaskManagement.WebUI.Controllers
{
    [ApiController]
    [Route("")]
    public class ToDoEventsController : BaseController
    {
        private readonly ILogger<ToDoEventsController> _logger;
        
        public ToDoEventsController(ILogger<ToDoEventsController> logger) {
            _logger = logger;
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Topic("pubsub", "placementEvents", "deadletters", false)]
        public async Task<ActionResult> ReceivePlacementEvent([FromBody] PlacementReservationCreated placementCreatedEvent)
        {
            _logger.LogInformation($"Receive placement event on Location:" + placementCreatedEvent.Location);
            #region Bad Request
            //return BadRequest();
            #endregion

            #region Transient Error
            //if (new Random().NextInt64() % 2 != 0)
            //{
            //    _logger.LogInformation($"BadRequest {placementCreatedEvent.Location} by {placementCreatedEvent.ObjectId}");
                //return StatusCode(503);
            //}
            #endregion

            #region Long time Failure
            //_logger.LogInformation($"BadRequest {placementReservation.Location} by {placementReservation.ObjectId}");
            //return StatusCode(503);
            #endregion

            await Mediator.Send(placementCreatedEvent);
            return Ok();
        }
    }
   
}
