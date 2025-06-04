using MovieStoreB.BL.Interfaces;
using MovieStoreB.DL.Cache;
using MovieStoreB.DL.Interfaces;
using MovieStoreB.Models.DTO;
using Microsoft.Extensions.Logging;

namespace MovieStoreB.BL.Services
{
    internal class ActorService : IActorService
    {
        private readonly IActorRepository _actorRepository;
        private readonly IInMemoryCacheService<Actor, string> _actorCacheService;
        private readonly ILogger<ActorService> _logger;

        public ActorService(
            IActorRepository actorRepository,
            IInMemoryCacheService<Actor, string> actorCacheService,
            ILogger<ActorService> logger)
        {
            _actorRepository = actorRepository;
            _actorCacheService = actorCacheService;
            _logger = logger;
        }

        public async Task<IEnumerable<Actor>> GetAllActors()
        {
            try
            {
                // First try cache
                var cachedActors = await _actorCacheService.GetAll();
                if (cachedActors.Any())
                {
                    _logger.LogInformation("Retrieved {Count} actors from cache", cachedActors.Count());
                    return cachedActors;
                }

                // If cache is empty, get from repository
                _logger.LogInformation("Cache is empty, retrieving actors from repository");
                var actors = await _actorRepository.GetAllActors();

                // Add to cache
                if (actors.Any())
                {
                    await _actorCacheService.AddOrUpdateBatch(actors);
                }

                return actors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving actors");
                throw;
            }
        }

        public async Task<Actor?> GetActorById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            try
            {
                // First try cache
                var cachedActor = await _actorCacheService.Get(id);
                if (cachedActor != null)
                {
                    _logger.LogDebug("Retrieved actor {ActorId} from cache", id);
                    return cachedActor;
                }

                // If not in cache, get from repository
                var actor = await _actorRepository.GetById(id);
                if (actor != null)
                {
                    // Add to cache
                    await _actorCacheService.AddOrUpdate(actor);
                }

                return actor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving actor {ActorId}", id);
                throw;
            }
        }

        public async Task AddActor(Actor actor)
        {
            if (actor == null) return;

            try
            {
                actor.DateInserted = DateTime.UtcNow;

                // Add to repository
                await _actorRepository.AddActor(actor);

                // Add to cache
                await _actorCacheService.AddOrUpdate(actor);

                _logger.LogInformation("Added actor {ActorId}", actor.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding actor {ActorId}", actor.Id);
                throw;
            }
        }

        public async Task UpdateActor(Actor actor)
        {
            if (actor == null || string.IsNullOrEmpty(actor.Id)) return;

            try
            {
                // Update in repository
                await _actorRepository.UpdateActor(actor);

                // Update cache
                await _actorCacheService.AddOrUpdate(actor);

                _logger.LogInformation("Updated actor {ActorId}", actor.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating actor {ActorId}", actor.Id);
                throw;
            }
        }

        public async Task DeleteActor(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            try
            {
                // Delete from repository
                await _actorRepository.DeleteActor(id);

                // Remove from cache
                await _actorCacheService.Remove(id);

                _logger.LogInformation("Deleted actor {ActorId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting actor {ActorId}", id);
                throw;
            }
        }
    }
}