using Microsoft.AspNetCore.Mvc;
using MovieStoreB.BL.Interfaces;
using MovieStoreB.Models.DTO;
using MovieStoreB.Models.Requests;

namespace MovieStoreB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesBlController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MoviesBlController> _logger;

        public MoviesBlController(
            IMovieService movieService,
            ILogger<MoviesBlController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

        [HttpPost("TestFluentValid")]
        public async Task<IActionResult> TestFluentValid([FromBody] TestRequest movieRequest)
        {
            return Ok();
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var movies = await _movieService.GetMovies();
                return Ok(movies);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in GetAll {e.Message}-{e.StackTrace}");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class TestRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}