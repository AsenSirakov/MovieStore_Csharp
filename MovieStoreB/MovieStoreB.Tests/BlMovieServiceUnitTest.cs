using Moq;
using MovieStoreB.BL.Interfaces;
using MovieStoreB.BL.Services;
using MovieStoreB.DL.Interfaces;
using MovieStoreB.Models.DTO;
using Microsoft.Extensions.Logging;

namespace MovieStoreB.Tests
{
    public class BlMovieServiceUnitTest
    {
        private readonly Mock<IMovieService> _movieServiceMock;
        private readonly Mock<IActorService> _actorServiceMock; // Changed from IActorRepository to IActorService
        private readonly Mock<ILogger<BlMovieService>> _loggerMock; // Added logger mock

        private List<Movie> _movies = new List<Movie>()
        {
            new Movie()
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Movie 1",
                Year = 2021,
                ActorIds = [
                    "157af604-7a4b-4538-b6a9-fed41a41cf3a",
                    "baac2b19-bbd2-468d-bd3b-5bd18aba98d7"]
            },
            new Movie()
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Movie 2",
                Year = 2022,
                ActorIds = [
                    "157af604-7a4b-4538-b6a9-fed41a41cf3a",
                    "5c93ba13-e803-49c1-b465-d471607e97b3"
                ]
            }
        };

        private List<Actor> _actors = new List<Actor>
        {
            new Actor("157af604-7a4b-4538-b6a9-fed41a41cf3a", "Actor 1"),
            new Actor("baac2b19-bbd2-468d-bd3b-5bd18aba98d7", "Actor 2"),
            new Actor("5c93ba13-e803-49c1-b465-d471607e97b3", "Actor 3"),
        };

        public BlMovieServiceUnitTest()
        {
            _movieServiceMock = new Mock<IMovieService>();
            _actorServiceMock = new Mock<IActorService>(); // Changed to IActorService
            _loggerMock = new Mock<ILogger<BlMovieService>>(); // Added logger mock
        }

        [Fact]
        public async void GetAllMovieDetails_ReturnsData()
        {
            //setup
            var expectedCount = 2;

            _movieServiceMock
                .Setup(x => x.GetMovies())
                .ReturnsAsync(_movies);

            // Changed to use IActorService.GetActorById instead of IActorRepository.GetById
            _actorServiceMock
                .Setup(service =>
                    service.GetActorById(It.IsAny<string>()))
                    .ReturnsAsync((string id) =>
                        _actors.FirstOrDefault(x => x.Id == id));

            //inject - Added logger to constructor
            var blMovieService = new BlMovieService(
                _movieServiceMock.Object,
                _actorServiceMock.Object,
                _loggerMock.Object);

            //act
            var result = await
                blMovieService.GetAllMovieDetails();

            //assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count);

            // Additional assertions to verify actors are included
            Assert.True(result.All(movie => movie.Actors != null));
            Assert.True(result.First().Actors.Count > 0); // Verify actors are actually added
        }
    }
}