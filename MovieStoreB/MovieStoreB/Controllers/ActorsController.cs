using Microsoft.AspNetCore.Mvc;
using MovieStoreB.BL.Interfaces;
using MovieStoreB.Models.DTO;

namespace MovieStoreB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActorsController : ControllerBase
    {
        private readonly IActorService _actorService;
        private readonly ILogger<ActorsController> _logger;

        public ActorsController(IActorService actorService, ILogger<ActorsController> logger)
        {
            _actorService = actorService;
            _logger = logger;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var actors = await _actorService.GetAllActors();
                return Ok(actors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all actors");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Actor ID is required");

                var actor = await _actorService.GetActorById(id);

                if (actor == null)
                    return NotFound($"Actor with ID {id} not found");

                return Ok(actor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting actor {ActorId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddActor([FromBody] AddActorRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Name))
                    return BadRequest("Actor name is required");

                var actor = new Actor(Guid.NewGuid().ToString(), request.Name);

                await _actorService.AddActor(actor);

                return Ok(new { message = "Actor added successfully", actorId = actor.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding actor");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateActor(string id, [FromBody] UpdateActorRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || request == null || string.IsNullOrEmpty(request.Name))
                    return BadRequest("Actor ID and name are required");

                var existingActor = await _actorService.GetActorById(id);
                if (existingActor == null)
                    return NotFound($"Actor with ID {id} not found");

                var updatedActor = new Actor(id, request.Name);
                await _actorService.UpdateActor(updatedActor);

                return Ok(new { message = "Actor updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating actor {ActorId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteActor(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Actor ID is required");

                var existingActor = await _actorService.GetActorById(id);
                if (existingActor == null)
                    return NotFound($"Actor with ID {id} not found");

                await _actorService.DeleteActor(id);

                return Ok(new { message = "Actor deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting actor {ActorId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class AddActorRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateActorRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}