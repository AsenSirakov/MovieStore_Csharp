using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieStoreB.BL.Interfaces
{
    public interface IExternalApiService
    {
        Task<List<ExternalMovieData>> GetMoviesFromExternalApi();
        Task<ExternalActorData?> GetActorFromExternalApi(string actorName);
        Task<bool> NotifyExternalSystemAsync(string movieId, string action);
    }

    // DTOs for external API data - put these here or in Models project
    public class ExternalMovieData
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Source { get; set; } = string.Empty;
    }

    public class ExternalActorData
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
    }

    // JSON DTOs for external APIs
    public class JsonPost
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

    public class JsonUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
    }
}