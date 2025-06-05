using MovieStoreB.DL.Interfaces;
using MovieStoreB.Models.DTO;
using MovieStoreB.Models.Responses;
using RestSharp;
using System.Text.Json;

namespace MovieStoreB.DL.Gateways
{
    public class ActorBioGateway : IActorBioGateway
    {
        private readonly RestClient _client;

        public ActorBioGateway()
        {
            // Using JSONPlaceholder as external API (like your teacher's setup)
            var options = new RestClientOptions("https://jsonplaceholder.typicode.com")
            {
                ThrowOnAnyError = false, // Don't throw exceptions on HTTP errors
                Timeout = TimeSpan.FromSeconds(30)
            };

            _client = new RestClient(options);
        }

        public async Task<ActorBioResponse> GetBioByActorId(string actorId)
        {
            try
            {
                // Create request using RestSharp's fluent API
                var request = new RestRequest($"/users/{actorId}", Method.Get);

                // Execute request and get typed response
                var response = await _client.ExecuteAsync<JsonPlaceholderUser>(request);

                if (response.IsSuccessful && response.Data != null)
                {
                    var user = response.Data;
                    return new ActorBioResponse
                    {
                        Summary = $"Actor Bio: {user.Name} ({user.Email}) - Works at {user.Company?.Name} - {user.Company?.CatchPhrase}"
                    };
                }

                return new ActorBioResponse
                {
                    Summary = $"Actor bio not found for ID: {actorId}"
                };
            }
            catch (Exception ex)
            {
                return new ActorBioResponse
                {
                    Summary = $"Error fetching actor bio: {ex.Message}"
                };
            }
        }

        public async Task<ActorBioResponse> GetBioByActor(Actor actor)
        {
            try
            {
                // Create POST request with JSON body
                var request = new RestRequest("/posts", Method.Post);

                // RestSharp automatically serializes to JSON
                request.AddJsonBody(new
                {
                    title = $"Biography for {actor.Name}",
                    body = $"Detailed biography and career information for actor {actor.Name} (ID: {actor.Id})",
                    userId = 1
                });

                // Execute and get typed response
                var response = await _client.ExecuteAsync<JsonPlaceholderPost>(request);

                if (response.IsSuccessful && response.Data != null)
                {
                    return new ActorBioResponse
                    {
                        Summary = $"Created biography entry #{response.Data.Id} for {actor.Name}: {response.Data.Title}"
                    };
                }

                return new ActorBioResponse
                {
                    Summary = $"Failed to create biography for {actor.Name}"
                };
            }
            catch (Exception ex)
            {
                return new ActorBioResponse
                {
                    Summary = $"Error creating actor bio: {ex.Message}"
                };
            }
        }

        // JSONPlaceholder response models
        public class JsonPlaceholderUser
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public string Phone { get; set; } = "";
            public string Website { get; set; } = "";
            public JsonPlaceholderCompany? Company { get; set; }
        }

        public class JsonPlaceholderCompany
        {
            public string Name { get; set; } = "";
            public string CatchPhrase { get; set; } = "";
            public string Bs { get; set; } = "";
        }

        public class JsonPlaceholderPost
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string Title { get; set; } = "";
            public string Body { get; set; } = "";
        }
    }
}