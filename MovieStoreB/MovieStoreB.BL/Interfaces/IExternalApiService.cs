using System.Collections.Generic;
using System.Threading.Tasks;
using MessagePack;

namespace MovieStoreB.BL.Interfaces
{
    public interface IExternalApiService
    {
        Task<List<ExternalMovieData>> GetMoviesFromExternalApi();
        Task<ExternalActorData?> GetActorFromExternalApi(string actorName);
        Task<bool> NotifyExternalSystemAsync(string movieId, string action);
    }

    // DTOs for external API data - ADD MessagePack attributes
    [MessagePackObject]
    public class ExternalMovieData
    {
        [Key(0)]
        public string ExternalId { get; set; } = string.Empty;

        [Key(1)]
        public string Title { get; set; } = string.Empty;

        [Key(2)]
        public string Description { get; set; } = string.Empty;

        [Key(3)]
        public int Year { get; set; }

        [Key(4)]
        public string Source { get; set; } = string.Empty;
    }

    [MessagePackObject]
    public class ExternalActorData
    {
        [Key(0)]
        public string ExternalId { get; set; } = string.Empty;

        [Key(1)]
        public string Name { get; set; } = string.Empty;

        [Key(2)]
        public string Email { get; set; } = string.Empty;

        [Key(3)]
        public string Website { get; set; } = string.Empty;

        [Key(4)]
        public string Source { get; set; } = string.Empty;
    }

    // JSON DTOs for external APIs - ADD MessagePack attributes
    [MessagePackObject]
    public class JsonPost
    {
        [Key(0)]
        public int UserId { get; set; }

        [Key(1)]
        public int Id { get; set; }

        [Key(2)]
        public string Title { get; set; } = string.Empty;

        [Key(3)]
        public string Body { get; set; } = string.Empty;
    }

    [MessagePackObject]
    public class JsonUser
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public string Name { get; set; } = string.Empty;

        [Key(2)]
        public string Username { get; set; } = string.Empty;

        [Key(3)]
        public string Email { get; set; } = string.Empty;

        [Key(4)]
        public string Phone { get; set; } = string.Empty;

        [Key(5)]
        public string Website { get; set; } = string.Empty;
    }
}