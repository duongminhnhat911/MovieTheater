using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MovieManagement.Data;
using MovieManagement.Models.Entities;
using MovieManagement.Models.DTOs;
using MovieManagementWeb_API.Controllers;

namespace MovieManagement.Tests
{
    public class MoviesControllerTests
    {
        private async Task<ApplicationDbContext> GetInMemoryDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed valid movies
            context.Movies.Add(CreateValidMovie(1, "Movie 1"));
            context.Movies.Add(CreateValidMovie(2, "Movie 2"));

            await context.SaveChangesAsync();
            return context;
        }

        private Movie CreateValidMovie(int id, string title)
        {
            return new Movie
            {
                Id = id,
                Title = title,
                ReleaseDate = DateTime.UtcNow,
                CarouselImage = "carousel.jpg",
                Description = "Description of movie",
                Director = "Director Name",
                Image = "image.jpg",
                ProductionCompany = "Studio",
                RatingCode = "PG-13",
                Subtitle = "English",
                TrailerLink = "https://youtube.com/trailer",
                MovieGenres = new List<MovieGenre>(),
                ActorMovies = new List<ActorMovie>(),
                Format = new List<string>(),
                Status = "Active"
            };
        }

        [Fact]
        public async Task GetMovies_ReturnsAllMovies()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var result = await controller.GetMovies();

            var okResult = Assert.IsType<ActionResult<IEnumerable<FilmDto>>>(result);
            var movies = Assert.IsType<List<FilmDto>>(okResult.Value);
            Assert.Equal(2, movies.Count);
        }

        [Fact]
        public async Task GetMovie_ValidId_ReturnsMovie()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var result = await controller.GetMovie(1);

            var okResult = Assert.IsType<ActionResult<FilmDto>>(result);
            Assert.Equal(1, okResult.Value.Id);
        }

        [Fact]
        public async Task GetMovie_InvalidId_ReturnsNotFound()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var result = await controller.GetMovie(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostMovie_ValidData_CreatesMovie()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var newMovie = new FilmDto
            {
                Title = "New Movie",
                ReleaseDate = DateTime.UtcNow,
                Format = new List<string>(),
                Genres = new List<string> { "Action" },
                Actors = new List<string> { "Actor A" },
                CarouselImage = "carousel.jpg",
                Description = "Great movie",
                Director = "Director 1",
                Image = "poster.jpg",
                ProductionCompany = "Studio 1",
                RatingCode = "PG",
                Subtitle = "English",
                TrailerLink = "https://example.com/trailer"
            };

            var result = await controller.PostMovie(newMovie);

            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<FilmDto>(createdAt.Value);
            Assert.Equal("New Movie", returnValue.Title);
        }


        [Fact]
        public async Task PutMovie_ValidId_UpdatesMovie()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var updateDto = new FilmDto
            {
                Id = 1,
                Title = "Updated Title",
                Format = new List<string>(),
                Genres = new List<string>(),
                Actors = new List<string>()
            };

            var result = await controller.PutMovie(1, updateDto);

            Assert.IsType<NoContentResult>(result);
            var movie = await context.Movies.FindAsync(1);
            Assert.Equal("Updated Title", movie.Title);
        }

        [Fact]
        public async Task PutMovie_InvalidId_ReturnsBadRequest()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var updateDto = new FilmDto { Id = 1, Title = "Mismatch" };

            var result = await controller.PutMovie(2, updateDto);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutMovie_NonexistentId_ReturnsNotFound()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var updateDto = new FilmDto { Id = 999, Title = "Not Exist" };

            var result = await controller.PutMovie(999, updateDto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteMovie_ValidId_MarksAsSoldOut()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var result = await controller.DeleteMovie(1);

            Assert.IsType<NoContentResult>(result);
            var movie = await context.Movies.FindAsync(1);
            Assert.Equal("SoldOut", movie.Status);
        }

        [Fact]
        public async Task DeleteMovie_InvalidId_ReturnsNotFound()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var result = await controller.DeleteMovie(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PostMovie_WithEmptyGenres_CreatesMovieWithoutGenres()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var dto = new FilmDto
            {
                Title = "No Genre Movie",
                ReleaseDate = DateTime.UtcNow,
                Format = new List<string>(),
                Genres = new List<string>(),
                Actors = new List<string>(),
                CarouselImage = "c.jpg",
                Description = "desc",
                Director = "dir",
                Image = "img.jpg",
                ProductionCompany = "studio",
                RatingCode = "PG",
                Subtitle = "en",
                TrailerLink = "link"
            };

            var result = await controller.PostMovie(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var movie = Assert.IsType<FilmDto>(created.Value);
            Assert.Empty(movie.Genres);
        }


        [Fact]
        public async Task PostMovie_WithEmptyActors_CreatesMovieWithoutActors()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var dto = new FilmDto
            {
                Title = "No Actor Movie",
                ReleaseDate = DateTime.UtcNow,
                Format = new List<string>(),
                Genres = new List<string> { "Drama" },
                Actors = new List<string>(),
                CarouselImage = "c.jpg",
                Description = "desc",
                Director = "dir",
                Image = "img.jpg",
                ProductionCompany = "studio",
                RatingCode = "PG",
                Subtitle = "en",
                TrailerLink = "link"
            };

            var result = await controller.PostMovie(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var movie = Assert.IsType<FilmDto>(created.Value);
            Assert.Empty(movie.Actors);
        }


        [Fact]
        public async Task PostMovie_NullReleaseDate_DefaultsToUtcNow()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var dto = new FilmDto
            {
                Title = "No Date",
                Format = new List<string>(),
                Genres = new List<string>(),
                Actors = new List<string>(),
                CarouselImage = "c.jpg",
                Description = "desc",
                Director = "dir",
                Image = "img.jpg",
                ProductionCompany = "studio",
                RatingCode = "PG",
                Subtitle = "en",
                TrailerLink = "link"
            };

            var result = await controller.PostMovie(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var movie = Assert.IsType<FilmDto>(created.Value);
            Assert.True((DateTime.UtcNow - movie.ReleaseDate.Value).TotalSeconds < 5);
        }


        [Fact]
        public async Task PutMovie_UpdatesGenresCorrectly()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var updateDto = new FilmDto
            {
                Id = 1,
                Title = "Updated",
                Format = new List<string>(),
                Genres = new List<string> { "Thriller" },
                Actors = new List<string>()
            };

            await controller.PutMovie(1, updateDto);

            var movie = await context.Movies.Include(m => m.MovieGenres).ThenInclude(g => g.Genre).FirstAsync(m => m.Id == 1);
            Assert.Single(movie.MovieGenres);
            Assert.Equal("Thriller", movie.MovieGenres.First().Genre.Name);
        }

        [Fact]
        public async Task PutMovie_UpdatesActorsCorrectly()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var updateDto = new FilmDto
            {
                Id = 1,
                Title = "Updated",
                Format = new List<string>(),
                Genres = new List<string>(),
                Actors = new List<string> { "Actor B" }
            };

            await controller.PutMovie(1, updateDto);

            var movie = await context.Movies.Include(m => m.ActorMovies).ThenInclude(a => a.Actor).FirstAsync(m => m.Id == 1);
            Assert.Single(movie.ActorMovies);
            Assert.Equal("Actor B", movie.ActorMovies.First().Actor.Name);
        }

        [Fact]
        public async Task MovieExists_ReturnsTrue_IfMovieExists()
        {
            var context = await GetInMemoryDbContextAsync();
            var exists = context.Movies.Any(m => m.Id == 1);
            Assert.True(exists);
        }

        [Fact]
        public async Task MovieExists_ReturnsFalse_IfMovieDoesNotExist()
        {
            var context = await GetInMemoryDbContextAsync();
            var exists = context.Movies.Any(m => m.Id == 999);
            Assert.False(exists);
        }

        [Fact]
        public async Task PostMovie_CreatesGenreIfNotExists()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var dto = new FilmDto
            {
                Title = "Sci-fi movie",
                Genres = new List<string> { "Sci-Fi" },
                Format = new List<string>(),
                Actors = new List<string>(),
                CarouselImage = "c.jpg",
                Description = "desc",
                Director = "dir",
                Image = "img.jpg",
                ProductionCompany = "studio",
                RatingCode = "PG",
                Subtitle = "en",
                TrailerLink = "link"
            };

            await controller.PostMovie(dto);

            var genre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Sci-Fi");
            Assert.NotNull(genre);
        }


        [Fact]
        public async Task PostMovie_CreatesActorIfNotExists()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var dto = new FilmDto
            {
                Title = "New Actor Movie",
                Actors = new List<string> { "New Actor" },
                Genres = new List<string>(),
                Format = new List<string>(),
                CarouselImage = "c.jpg",
                Description = "desc",
                Director = "dir",
                Image = "img.jpg",
                ProductionCompany = "studio",
                RatingCode = "PG",
                Subtitle = "en",
                TrailerLink = "link"
            };

            await controller.PostMovie(dto);

            var actor = await context.Actors.FirstOrDefaultAsync(a => a.Name == "New Actor");
            Assert.NotNull(actor);
        }


        [Fact]
        public async Task PutMovie_KeepsUnchangedProperties()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var before = await context.Movies.FindAsync(1);

            var dto = new FilmDto
            {
                Id = 1,
                Title = before.Title,
                Format = before.Format,
                Genres = new List<string>(),
                Actors = new List<string>()
            };

            await controller.PutMovie(1, dto);

            var after = await context.Movies.FindAsync(1);
            Assert.Equal(before.Title, after.Title);
        }

        [Fact]
        public async Task PostMovie_WithMultipleGenresAndActors_Success()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var dto = new FilmDto
            {
                Title = "Ensemble Film",
                Genres = new List<string> { "Action", "Drama" },
                Actors = new List<string> { "Actor 1", "Actor 2" },
                Format = new List<string>(),
                CarouselImage = "c.jpg",
                Description = "desc",
                Director = "dir",
                Image = "img.jpg",
                ProductionCompany = "studio",
                RatingCode = "PG",
                Subtitle = "en",
                TrailerLink = "link"
            };

            var result = await controller.PostMovie(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var movie = Assert.IsType<FilmDto>(created.Value);
            Assert.Equal(2, movie.Genres.Count);
            Assert.Equal(2, movie.Actors.Count);
        }

        [Fact]
        public async Task PostMovie_WithNullActorsAndGenres_DoesNotThrow()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var dto = new FilmDto
            {
                Title = "Null Case",
                Genres = null,
                Actors = null,
                Format = new List<string>(),
                CarouselImage = "c.jpg",
                Description = "desc",
                Director = "dir",
                Image = "img.jpg",
                ProductionCompany = "studio",
                RatingCode = "PG",
                Subtitle = "en",
                TrailerLink = "link"
            };

            var result = await controller.PostMovie(dto);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.NotNull(created.Value);
        }

        [Fact]
        public async Task PutMovie_WithNullActorsAndGenres_DoesNotThrow()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var dto = new FilmDto
            {
                Id = 1,
                Title = "Null Update",
                Genres = null,
                Actors = null,
                Format = new List<string>()
            };

            var result = await controller.PutMovie(1, dto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetMovie_ReturnsNotFound_WhenMovieDoesNotExist()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var result = await controller.GetMovie(999); // ID không tồn tại

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PutMovie_ReturnsBadRequest_WhenIdMismatch()
        {
            var context = await GetInMemoryDbContextAsync();
            var controller = new MoviesController(context);

            var dto = new FilmDto
            {
                Title = "Updated Movie",
                ReleaseDate = DateTime.UtcNow,
                Format = new List<string>(),
                Genres = new List<string>(),
                Actors = new List<string>()
            };

            var result = await controller.PutMovie(5, dto); // ID không khớp DTO.Id (vì DTO không có Id)

            Assert.IsType<BadRequestResult>(result);
        }
    }
}
