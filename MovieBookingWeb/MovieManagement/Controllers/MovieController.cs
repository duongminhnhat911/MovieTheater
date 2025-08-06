using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagement.Data;
using MovieManagement.Models.DTOs;
using MovieManagement.Models.Entities;


namespace MovieManagementWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FilmDto>>> GetMovies()
        {
            var movies = await _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.ActorMovies)
                    .ThenInclude(am => am.Actor)
                .ToListAsync();

            var filmDtos = movies.Select(m => new FilmDto
            {
                Id = m.Id,
                Title = m.Title,
                Image = m.Image,
                CarouselImage = m.CarouselImage,
                TrailerLink = m.TrailerLink,
                Subtitle = m.Subtitle,
                RatingCode = m.RatingCode,
                Duration = m.Duration,
                Director = m.Director,
                Description = m.Description,
                ProductionCompany = m.ProductionCompany,
                Format = m.Format,
                ReleaseDate = m.ReleaseDate,
                Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                Actors = m.ActorMovies.Select(am => am.Actor.Name).ToList(),
                CreatedByUsername = m.CreatedByUsername,
                EditedByUsername = m.EditedByUsername,
                Status = m.Status
            }).ToList();

            return filmDtos;
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FilmDto>> GetMovie(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.ActorMovies)
                    .ThenInclude(am => am.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var filmDto = new FilmDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Image = movie.Image,
                CarouselImage = movie.CarouselImage,
                TrailerLink = movie.TrailerLink,
                Subtitle = movie.Subtitle,
                RatingCode = movie.RatingCode,
                Duration = movie.Duration,
                Director = movie.Director,
                Description = movie.Description,
                ProductionCompany = movie.ProductionCompany,
                Format = movie.Format,
                ReleaseDate = movie.ReleaseDate,
                Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                Actors = movie.ActorMovies.Select(am => am.Actor.Name).ToList(),
                CreatedByUsername = movie.CreatedByUsername,
                EditedByUsername = movie.EditedByUsername,
                Status = movie.Status
            };

            return filmDto;
        }

        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(FilmDto filmDto)
        {
            var movie = new Movie
            {
                Title = filmDto.Title,
                Image = filmDto.Image,
                CarouselImage = filmDto.CarouselImage,
                TrailerLink = filmDto.TrailerLink,
                Subtitle = filmDto.Subtitle,
                RatingCode = filmDto.RatingCode,
                Duration = filmDto.Duration,
                Director = filmDto.Director,
                Description = filmDto.Description,
                ProductionCompany = filmDto.ProductionCompany,
                Format = filmDto.Format,
                ReleaseDate = DateTime.SpecifyKind(filmDto.ReleaseDate?.Date ?? DateTime.Today, DateTimeKind.Utc),
                MovieGenres = new List<MovieGenre>(),
                ActorMovies = new List<ActorMovie>(),
                CreatedByUsername = "admin",
                Status = filmDto.Status ?? "Active"
            };

            // Handle genres
            if (filmDto.Genres != null)
            {
                foreach (var genreName in filmDto.Genres)
                {
                    var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
                    if (genre == null)
                    {
                        genre = new Genre { Name = genreName };
                        _context.Genres.Add(genre);
                    }
                    movie.MovieGenres.Add(new MovieGenre { Genre = genre });
                }
            }

            // Handle actors
            if (filmDto.Actors != null)
            {
                foreach (var actorName in filmDto.Actors)
                {
                    var actor = await _context.Actors.FirstOrDefaultAsync(a => a.Name == actorName);
                    if (actor == null)
                    {
                        actor = new Actor { Name = actorName };
                        _context.Actors.Add(actor);
                    }
                    movie.ActorMovies.Add(new ActorMovie { Actor = actor });
                }
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, new FilmDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Image = movie.Image,
                CarouselImage = movie.CarouselImage,
                TrailerLink = movie.TrailerLink,
                Subtitle = movie.Subtitle,
                RatingCode = movie.RatingCode,
                Duration = movie.Duration,
                Director = movie.Director,
                Description = movie.Description,
                ProductionCompany = movie.ProductionCompany,
                Format = movie.Format,
                ReleaseDate = movie.ReleaseDate,
                Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                Actors = movie.ActorMovies.Select(am => am.Actor.Name).ToList(),
                CreatedByUsername = movie.CreatedByUsername,
                EditedByUsername = movie.EditedByUsername,
                Status = movie.Status
            });

        }

        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, FilmDto filmDto)
        {
            if (id != filmDto.Id)
                return BadRequest();

            var movie = await _context.Movies
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.ActorMovies).ThenInclude(am => am.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            // Update simple properties
            movie.Title = filmDto.Title;
            movie.Image = filmDto.Image;
            movie.CarouselImage = filmDto.CarouselImage;
            movie.TrailerLink = filmDto.TrailerLink;
            movie.Subtitle = filmDto.Subtitle;
            movie.RatingCode = filmDto.RatingCode;
            movie.Duration = filmDto.Duration;
            movie.Director = filmDto.Director;
            movie.Description = filmDto.Description;
            movie.ProductionCompany = filmDto.ProductionCompany;
            movie.Format = filmDto.Format;
            movie.ReleaseDate = DateTime.SpecifyKind(filmDto.ReleaseDate?.Date ?? DateTime.Today, DateTimeKind.Utc);
            movie.EditedByUsername = filmDto.EditedByUsername;
            movie.Status = filmDto.Status ?? movie.Status;

            // Update Genres
            var newGenreNames = filmDto.Genres ?? new List<string>();
            var currentGenres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList();
            var genresToRemove = movie.MovieGenres.Where(mg => !newGenreNames.Contains(mg.Genre.Name)).ToList();
            var genresToAdd = newGenreNames.Where(name => !currentGenres.Contains(name)).ToList();

            _context.MovieGenres.RemoveRange(genresToRemove);

            foreach (var genreName in genresToAdd)
            {
                var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
                if (genre == null)
                {
                    genre = new Genre { Name = genreName };
                    _context.Genres.Add(genre);
                }
                movie.MovieGenres.Add(new MovieGenre { MovieId = movie.Id, Genre = genre });
            }

            // Update Actors
            var newActorNames = filmDto.Actors ?? new List<string>();
            var currentActorNames = movie.ActorMovies.Select(am => am.Actor.Name).ToList();
            var actorsToRemove = movie.ActorMovies.Where(am => !newActorNames.Contains(am.Actor.Name)).ToList();
            var actorNamesToAdd = newActorNames.Where(name => !currentActorNames.Contains(name)).ToList();

            _context.ActorMovies.RemoveRange(actorsToRemove);

            foreach (var actorName in actorNamesToAdd)
            {
                var actor = await _context.Actors.FirstOrDefaultAsync(a => a.Name == actorName);
                if (actor == null)
                {
                    actor = new Actor { Name = actorName };
                    _context.Actors.Add(actor);
                }
                movie.ActorMovies.Add(new ActorMovie { MovieId = movie.Id, Actor = actor });
            }

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            movie.Status = "SoldOut"; // soft delete
            _context.Entry(movie).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
