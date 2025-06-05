using Microsoft.AspNetCore.Mvc;
using MovieStoreB.BL.Interfaces;
using MovieStoreB.DL.Cache;
using MovieStoreB.Models.DTO;

namespace MovieStoreB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExternalApiController : ControllerBase
    {
        private readonly IExternalApiService _externalApiService;
        private readonly IMovieService _movieService;
        private readonly IInMemoryCacheService<Movie, string> _movieCacheService;
        private readonly IInMemoryCacheService<Actor, string> _actorCacheService;
        private readonly ILogger<ExternalApiController> _logger;

        public ExternalApiController(
            IExternalApiService externalApiService,
            IMovieService movieService,
            IInMemoryCacheService<Movie, string> movieCacheService,
            IInMemoryCacheService<Actor, string> actorCacheService,
            ILogger<ExternalApiController> logger)
        {
            _externalApiService = externalApiService;
            _movieService = movieService;
            _movieCacheService = movieCacheService;
            _actorCacheService = actorCacheService;
            _logger = logger;
        }

        [HttpGet("import-movies")]
        public async Task<IActionResult> ImportMoviesFromExternalApi()
        {
            try
            {
                // Use the interface method
                await _movieService.ImportMoviesFromExternalApi();
                return Ok(new { message = "Successfully imported movies from external API" });
            }
            catch (NotImplementedException)
            {
                // Fallback to manual import if using basic MovieService
                return await ImportMoviesManually();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing movies from external API");
                return StatusCode(500, new { error = "Failed to import movies" });
            }
        }

        private async Task<IActionResult> ImportMoviesManually()
        {
            try
            {
                var externalMovies = await _externalApiService.GetMoviesFromExternalApi();

                foreach (var externalMovie in externalMovies)
                {
                    var movie = new Movie
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = externalMovie.Title,
                        Year = externalMovie.Year,
                        ActorIds = new List<string>(),
                        DateInserted = DateTime.UtcNow
                    };

                    await _movieService.AddMovie(movie);
                }

                return Ok(new { message = $"Successfully imported {externalMovies.Count} movies from external API" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in manual movie import");
                return StatusCode(500, new { error = "Failed to import movies manually" });
            }
        }

        [HttpGet("external-movies")]
        public async Task<IActionResult> GetExternalMovies()
        {
            try
            {
                var externalMovies = await _externalApiService.GetMoviesFromExternalApi();
                return Ok(externalMovies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching external movies");
                return StatusCode(500, new { error = "Failed to fetch external movies" });
            }
        }

        [HttpGet("external-actor/{actorName}")]
        public async Task<IActionResult> GetExternalActor(string actorName)
        {
            try
            {
                var actorData = await _externalApiService.GetActorFromExternalApi(actorName);
                if (actorData == null)
                {
                    return NotFound(new { message = $"No external data found for actor: {actorName}" });
                }
                return Ok(actorData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching external actor data for {ActorName}", actorName);
                return StatusCode(500, new { error = "Failed to fetch external actor data" });
            }
        }

        [HttpPost("notify/{movieId}/{action}")]
        public async Task<IActionResult> NotifyExternalSystem(string movieId, string action)
        {
            try
            {
                var success = await _externalApiService.NotifyExternalSystemAsync(movieId, action);
                if (success)
                {
                    return Ok(new { message = "External system notified successfully" });
                }
                return BadRequest(new { error = "Failed to notify external system" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying external system");
                return StatusCode(500, new { error = "Failed to notify external system" });
            }
        }

        [HttpGet("cache-status")]
        public async Task<IActionResult> GetCacheStatus()
        {
            try
            {
                var movieCount = _movieCacheService.Count;
                var actorCount = _actorCacheService.Count;

                return Ok(new
                {
                    moviesCached = movieCount,
                    actorsCached = actorCount,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache status");
                return StatusCode(500, new { error = "Failed to get cache status" });
            }
        }

        [HttpGet("cached-movies")]
        public async Task<IActionResult> GetCachedMovies()
        {
            try
            {
                var cachedMovies = await _movieCacheService.GetAll();
                return Ok(cachedMovies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cached movies");
                return StatusCode(500, new { error = "Failed to get cached movies" });
            }
        }

        [HttpGet("cached-actors")]
        public async Task<IActionResult> GetCachedActors()
        {
            try
            {
                var cachedActors = await _actorCacheService.GetAll();
                return Ok(cachedActors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cached actors");
                return StatusCode(500, new { error = "Failed to get cached actors" });
            }
        }

        [HttpDelete("clear-cache")]
        public async Task<IActionResult> ClearCache()
        {
            try
            {
                await _movieCacheService.Clear();
                await _actorCacheService.Clear();
                return Ok(new { message = "Cache cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cache");
                return StatusCode(500, new { error = "Failed to clear cache" });
            }
        }
    }
}