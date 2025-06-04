using Moq;
using MovieStoreB.BL.Interfaces;
using MovieStoreB.BL.Services;
using MovieStoreB.DL.Interfaces;
using MovieStoreB.Models.DTO;

namespace MovieStoreB.Tests
{
    public class MovieServiceTests
    {
        private readonly Mock<IMovieRepository> _movieRepositoryMock;
        private readonly Mock<IActorRepository> _actorRepositoryMock;

        private List<Movie> _movies = new List<Movie>()
        {
            new Movie()
            {
                Id = "c3bd1985-792e-4208-af81-4d154bff15c8",
                Title = "Movie 1",
                Year = 2021,
                ActorIds = [
                    "157af604-7a4b-4538-b6a9-fed41a41cf3a",
                    "baac2b19-bbd2-468d-bd3b-5bd18aba98d7"]
            },
            new Movie()
            {
                Id = "4c304bec-f213-47b5-8ae0-9df4a4eb3b99",
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

        public MovieServiceTests()
        {
            _actorRepositoryMock = new Mock<IActorRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
        }

        [Fact]
        async Task GetMoviesById_ReturnsData() // Made async
        {
            // Arrange
            var movieId = _movies[0].Id;

            _movieRepositoryMock.Setup(x => x.GetMoviesById(It.IsAny<string>()))
                    .ReturnsAsync((string id) => // Changed to ReturnsAsync
                        _movies.FirstOrDefault(x => x.Id == id));

            var movieService = new MovieService(_movieRepositoryMock.Object, _actorRepositoryMock.Object);

            // Act
            var result = await movieService.GetMoviesById(movieId); // Added await

            // Assert
            Assert.NotNull(result);
            Assert.Equal(movieId, result.Id);
        }

        [Fact]
        async Task GetMoviesById_MovieNotExist() // Made async
        {
            // Arrange
            var movieId = "c3bd1985-792e-4208-af81-4d154bff15c9";

            _movieRepositoryMock.Setup(x => x.GetMoviesById(It.IsAny<string>()))
                    .ReturnsAsync((string id) => // Changed to ReturnsAsync
                        _movies.FirstOrDefault(x => x.Id == id));

            var movieService = new MovieService(_movieRepositoryMock.Object, _actorRepositoryMock.Object);

            // Act
            var result = await movieService.GetMoviesById(movieId); // Added await

            // Assert
            Assert.Null(result);
        }

        [Fact]
        async Task GetMoviesById_MovieWithInvalidGuid() // Made async
        {
            // Arrange
            var movieId = "c3bd1985-792e-4208-af81-4d154bff15c9-12";

            _movieRepositoryMock.Setup(x => x.GetMoviesById(It.IsAny<string>()))
                    .ReturnsAsync((string id) => // Changed to ReturnsAsync
                        _movies.First(x => x.Id == id));

            var movieService = new MovieService(_movieRepositoryMock.Object, _actorRepositoryMock.Object);

            // Act
            var result = await movieService.GetMoviesById(movieId); // Added await

            // Assert
            Assert.Null(result);
        }

        [Fact]
        async Task AddMovie_ValidMovie_AddsSuccessfully() // New test for async AddMovie
        {
            // Arrange
            var newMovie = new Movie
            {
                Title = "New Movie",
                Year = 2024,
                ActorIds = ["157af604-7a4b-4538-b6a9-fed41a41cf3a"]
            };

            _movieRepositoryMock.Setup(x => x.AddMovie(It.IsAny<Movie>()))
                .Returns(Task.CompletedTask);

            var movieService = new MovieService(_movieRepositoryMock.Object, _actorRepositoryMock.Object);

            // Act
            await movieService.AddMovie(newMovie);

            // Assert
            _movieRepositoryMock.Verify(x => x.AddMovie(It.IsAny<Movie>()), Times.Once);
        }

        [Fact]
        async Task DeleteMovie_ValidId_DeletesSuccessfully() // New test for async DeleteMovie
        {
            // Arrange
            var movieId = "c3bd1985-792e-4208-af81-4d154bff15c8";

            _movieRepositoryMock.Setup(x => x.DeleteMovie(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var movieService = new MovieService(_movieRepositoryMock.Object, _actorRepositoryMock.Object);

            // Act
            await movieService.DeleteMovie(movieId);

            // Assert
            _movieRepositoryMock.Verify(x => x.DeleteMovie(movieId), Times.Once);
        }
    }
}