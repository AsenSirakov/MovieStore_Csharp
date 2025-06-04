using MovieStoreB.BL.Interfaces;
using MovieStoreB.DL.Cache;
using MovieStoreB.DL.Interfaces;
using MovieStoreB.Models.DTO;
using Microsoft.Extensions.Logging;

namespace MovieStoreB.BL.Services
{
    public class EnhancedMovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IActorRepository _actorRepository;
        private readonly IExternalApiService _externalApiService;
        private readonly IInMemoryCacheService<Movie, string> _movieCacheService;
        private readonly IInMemoryCacheService<Actor, string> _actorCacheService;
        private readonly ILogger<EnhancedMovieService> _logger;

        public EnhancedMovieService(
            IMovieRepository movieRepository,
            IActorRepository actorRepository,
            IExternalApiService externalApiService,
            IInMemoryCacheService<Movie, string> movieCacheService,
            IInMemoryCacheService<Actor, string> actorCacheService,
            ILogger<EnhancedMovieService> logger)
        {
            _movieRepository = movieRepository;
            _actorRepository = actorRepository;
            _externalApiService = externalApiService;
            _movieCacheService = movieCacheService;
            _actorCacheService = actorCacheService;
            _logger = logger;
        }

        public async Task<List<Movie>> GetMovies()
        {
            try
            {
                // First try to get from cache
                var cachedMovies = await _movieCacheService.GetAll();
                if (cachedMovies.Any())
                {
                    _logger.LogInformation("Retrieved {Count} movies from cache", cachedMovies.Count());
                    return cachedMovies.ToList();
                }

                // If cache is empty, get from repository
                _logger.LogInformation("Cache is empty, retrieving movies from repository");
                return await _movieRepository.GetMovies();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movies");
                throw;
            }
        }

        public async Task AddMovie(Movie movie)
        {
            if (movie == null || movie.ActorIds == null) return;

            movie.DateInserted = DateTime.UtcNow;

            foreach (var actor in movie.ActorIds)
            {
                if (!Guid.TryParse(actor, out _)) return;
            }

            await _movieRepository.AddMovie(movie);

            // Update cache
            await _movieCacheService.AddOrUpdate(movie);

            // Notify external system
            await _externalApiService.NotifyExternalSystemAsync(movie.Id, "MOVIE_ADDED");

            _logger.LogInformation("Added movie {MovieId} and notified external systems", movie.Id);
        }

        public async Task DeleteMovie(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            await _movieRepository.DeleteMovie(id);

            // Remove from cache
            await _movieCacheService.Remove(id);

            // Notify external system
            await _externalApiService.NotifyExternalSystemAsync(id, "MOVIE_DELETED");

            _logger.LogInformation("Deleted movie {MovieId} and notified external systems", id);
        }

        public async Task<Movie?> GetMoviesById(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var movieId))
            {
                return null;
            }

            // First try cache
            var cachedMovie = await _movieCacheService.Get(id);
            if (cachedMovie != null)
            {
                _logger.LogDebug("Retrieved movie {MovieId} from cache", id);
                return cachedMovie;
            }

            // If not in cache, get from repository
            var movie = await _movieRepository.GetMoviesById(movieId.ToString());
            if (movie != null)
            {
                // Add to cache for future requests
                await _movieCacheService.AddOrUpdate(movie);
            }

            return movie;
        }

        public async Task AddActor(string movieId, Actor actor)
        {
            if (string.IsNullOrEmpty(movieId) || actor == null) return;

            if (!Guid.TryParse(movieId, out _)) return;

            var movie = await GetMoviesById(movieId);

            if (movie == null) return;

            if (movie.ActorIds == null)
            {
                movie.ActorIds = new List<string>();
            }

            if (actor.Id == null || string.IsNullOrEmpty(actor.Id) || !Guid.TryParse(actor.Id, out _)) return;

            var existingActor = await _actorRepository.GetById(actor.Id);

            if (existingActor == null)
            {
                await _actorRepository.AddActor(actor);
            }

            if (!movie.ActorIds.Contains(actor.Id))
            {
                movie.ActorIds.Add(actor.Id);
                await _movieCacheService.AddOrUpdate(movie);
            }

            _logger.LogInformation("Added actor {ActorId} to movie {MovieId}", actor.Id, movieId);
        }

        public async Task ImportMoviesFromExternalApi()
        {
            try
            {
                _logger.LogInformation("Starting import of movies from external API");

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

                    await AddMovie(movie);
                }

                _logger.LogInformation("Successfully imported {Count} movies from external API", externalMovies.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing movies from external API");
            }
        }
    }
}