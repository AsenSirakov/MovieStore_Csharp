using Microsoft.Extensions.Logging;
using MovieStoreB.BL.Interfaces;
using System.Text.Json;

namespace MovieStoreB.BL.Services
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalApiService> _logger;

        public ExternalApiService(HttpClient httpClient, ILogger<ExternalApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ExternalMovieData>> GetMoviesFromExternalApi()
        {
            try
            {
                _logger.LogInformation("Fetching movies from external API");

                // Using JSONPlaceholder as an example external API
                var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/posts");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var posts = JsonSerializer.Deserialize<List<JsonPost>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Transform external data to our movie format
                    var movies = posts?.Take(10).Select(post => new ExternalMovieData
                    {
                        ExternalId = post.Id.ToString(),
                        Title = post.Title,
                        Description = post.Body,
                        Year = DateTime.Now.Year - (post.Id % 30), // Random year simulation
                        Source = "JSONPlaceholder API"
                    }).ToList() ?? new List<ExternalMovieData>();

                    _logger.LogInformation("Successfully fetched {Count} movies from external API", movies.Count);
                    return movies;
                }
                else
                {
                    _logger.LogWarning("Failed to fetch movies from external API. Status: {StatusCode}", response.StatusCode);
                    return new List<ExternalMovieData>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movies from external API");
                return new List<ExternalMovieData>();
            }
        }

        public async Task<ExternalActorData?> GetActorFromExternalApi(string actorName)
        {
            try
            {
                _logger.LogInformation("Fetching actor data for {ActorName} from external API", actorName);

                // Using JSONPlaceholder users as example actor data
                var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<JsonUser>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Find a user that somewhat matches the actor name or use first user
                    var user = users?.FirstOrDefault(u =>
                        u.Name.Contains(actorName, StringComparison.OrdinalIgnoreCase)) ?? users?.FirstOrDefault();

                    if (user != null)
                    {
                        var actorData = new ExternalActorData
                        {
                            ExternalId = user.Id.ToString(),
                            Name = user.Name,
                            Email = user.Email,
                            Website = user.Website,
                            Source = "JSONPlaceholder API"
                        };

                        _logger.LogInformation("Successfully fetched actor data for {ActorName}", actorName);
                        return actorData;
                    }
                }

                _logger.LogWarning("No actor data found for {ActorName}", actorName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching actor data for {ActorName}", actorName);
                return null;
            }
        }

        public async Task<bool> NotifyExternalSystemAsync(string movieId, string action)
        {
            try
            {
                _logger.LogInformation("Notifying external system about movie {MovieId} action {Action}", movieId, action);

                var notificationData = new
                {
                    MovieId = movieId,
                    Action = action,
                    Timestamp = DateTime.UtcNow,
                    Source = "MovieStoreB"
                };

                var json = JsonSerializer.Serialize(notificationData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // Using httpbin.org as example webhook endpoint
                var response = await _httpClient.PostAsync("https://httpbin.org/post", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully notified external system about movie {MovieId}", movieId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to notify external system. Status: {StatusCode}", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying external system about movie {MovieId}", movieId);
                return false;
            }
        }
    }
}