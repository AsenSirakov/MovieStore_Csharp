using MovieStoreB.Models.DTO;

namespace MovieStoreB.BL.Interfaces
{
    public interface IEnhancedMovieService : IMovieService
    {
        Task ImportMoviesFromExternalApi();
        Task<List<Movie>> GetMoviesFromCache();
        Task RefreshMovieCache();
    }
}