using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.WebUI.Controllers.Base;
using System.Threading.Tasks;
using Dapr;
using TaskManagement.Application.Remote;
using Microsoft.Extensions.Logging;

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
            _logger.LogInformation("Received placement");
            await Mediator.Send(placementCreatedEvent);
            return Ok();
        }
    }
   
}
