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
            
            #region Transient Error
            //if (new Random().NextInt64() % 3 == 0)
            //{
            //    _logger.LogInformation($"Failed to process  {placementCreatedEvent.Location} by {placementCreatedEvent.ObjectId}");
            //    return StatusCode(503);
            //}
            #endregion

            #region Long time Failure
            //_logger.LogInformation($"Failed to process {placementCreatedEvent.Location} by {placementCreatedEvent.ObjectId}");
            //return StatusCode(500);
            #endregion

            await Mediator.Send(placementCreatedEvent);
            return Ok();
        }
    }
   
}
