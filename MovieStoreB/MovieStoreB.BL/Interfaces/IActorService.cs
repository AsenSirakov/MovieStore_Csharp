using MovieStoreB.Models.DTO;

namespace MovieStoreB.BL.Interfaces
{
    public interface IActorService
    {
        Task<IEnumerable<Actor>> GetAllActors();
        Task<Actor?> GetActorById(string id);
        Task AddActor(Actor actor);
        Task UpdateActor(Actor actor);
        Task DeleteActor(string id);
    }
}