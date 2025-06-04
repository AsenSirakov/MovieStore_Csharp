using MovieStoreB.BL.Interfaces;
using MovieStoreB.DL.Cache;
using MovieStoreB.DL.Interfaces;
using MovieStoreB.Models.Responses;
using MovieStoreB.Models.DTO;
using Microsoft.Extensions.Logging;

namespace MovieStoreB.BL.Services
{
    internal class BlMovieService : IBlMovieService
    {
        private readonly IMovieService _movieService;
        private readonly IActorService _actorService; // Use ActorService instead of repository directly
        private readonly ILogger<BlMovieService> _logger;

        public BlMovieService(
            IMovieService movieService,
            IActorService actorService,
            ILogger<BlMovieService> logger)
        {
            _movieService = movieService;
            _actorService = actorService;
            _logger = logger;
        }

        public async Task<List<FullMovieDetails>> GetAllMovieDetails()
        {
            var result = new List<FullMovieDetails>();

            try
            {
                var movies = await _movieService.GetMovies();
                _logger.LogInformation("Retrieved {Count} movies for full details", movies.Count);

                foreach (var movie in movies)
                {
                    var movieDetails = new FullMovieDetails
                    {
                        Title = movie.Title,
                        Year = movie.Year,
                        Id = movie.Id,
                        DateInserted = movie.DateInserted,
                        Actors = new List<Actor>()
                    };

                    // FIXED: Actually add actors to the result
                    if (movie.ActorIds?.Any() == true)
                    {
                        foreach (var actorId in movie.ActorIds)
                        {
                            var actor = await _actorService.GetActorById(actorId);

                            if (actor != null)
                            {
                                movieDetails.Actors.Add(actor); // ← NOW actors are actually added!
                                _logger.LogDebug("Added actor {ActorName} to movie {MovieTitle}", actor.Name, movie.Title);
                            }
                            else
                            {
                                _logger.LogWarning("Actor with ID {ActorId} not found for movie {MovieId}", actorId, movie.Id);
                            }
                        }
                    }

                    result.Add(movieDetails);
                }

                _logger.LogInformation("Successfully created full details for {Count} movies with actors", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all movie details");
                throw;
            }
        }
    }
}