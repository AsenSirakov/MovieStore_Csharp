using MovieStoreB.DL.Cache;
using MovieStoreB.Models.DTO;

namespace MovieStoreB.DL.Interfaces
{
    public interface IMovieRepository : ICacheRepository<Movie>
    {
        Task<List<Movie>> GetMovies();

        Task AddMovie(Movie movie);

        Task DeleteMovie(string id);

        Task<Movie?> GetMoviesById(string id);

        Task UpdateMovie(Movie movie); // Added missing method

        Task<IEnumerable<Movie?>> GetMoviesAfterDateTime(DateTime date);
    }
}