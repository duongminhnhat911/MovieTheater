using Microsoft.AspNetCore.Mvc;
using MovieManagementWeb_API.Data;
using MovieManagementWeb_API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieManagementWeb_API.Models.DTOs;

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
                Cast = m.Cast,
                Description = m.Description,
                ProductionCompany = m.ProductionCompany,
                Format = m.Format,
                ReleaseDate = m.ReleaseDate,
                Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                Showtimes = null, // You can map showtimes if needed
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
                Cast = movie.Cast,
                Description = movie.Description,
                ProductionCompany = movie.ProductionCompany,
                Format = movie.Format,
                ReleaseDate = movie.ReleaseDate,
                Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                Showtimes = null, // You can map showtimes if needed
                CreatedByUsername = movie.CreatedByUsername,
                EditedByUsername = movie.EditedByUsername
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
                Cast = filmDto.Cast,
                Description = filmDto.Description,
                ProductionCompany = filmDto.ProductionCompany,
                Format = filmDto.Format,
                ReleaseDate = (filmDto.ReleaseDate ?? DateTime.Now).ToUniversalTime(),
                MovieGenres = new List<MovieGenre>(),
                Showtimes = filmDto.Showtimes?.Select(s => new Showtime { Date = s.Date.ToUniversalTime(), Time = s.Time, RoomName = s.RoomName }).ToList() ?? new List<Showtime>(),
                CreatedByUsername = "admin",
                Status = filmDto.Status ?? "Active"
            };

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

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, FilmDto filmDto)
        {
            if (id != filmDto.Id)
            {
                return BadRequest();
            }

            var movie = await _context.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Simple property mapping
            movie.Title = filmDto.Title;
            movie.Image = filmDto.Image;
            movie.CarouselImage = filmDto.CarouselImage;
            movie.TrailerLink = filmDto.TrailerLink;
            movie.Subtitle = filmDto.Subtitle;
            movie.RatingCode = filmDto.RatingCode;
            movie.Duration = filmDto.Duration;
            movie.Director = filmDto.Director;
            movie.Cast = filmDto.Cast;
            movie.Description = filmDto.Description;
            movie.ProductionCompany = filmDto.ProductionCompany;
            movie.Format = filmDto.Format;
            movie.ReleaseDate = (filmDto.ReleaseDate ?? movie.ReleaseDate).ToUniversalTime();
            movie.Status = filmDto.Status ?? movie.Status;

            // Update Showtimes
            if (movie.Showtimes != null)
                _context.Showtimes.RemoveRange(movie.Showtimes);
            if(filmDto.Showtimes != null)
            {
                movie.Showtimes = filmDto.Showtimes.Select(s => new Showtime { Date = s.Date.ToUniversalTime(), Time = s.Time, RoomName = s.RoomName }).ToList();
            }

            // Map EditedByUsername
            movie.EditedByUsername = filmDto.EditedByUsername;

            // Update genres
            var newGenreNames = filmDto.Genres ?? new List<string>();
            var currentGenreNames = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList();

            var genresToRemove = movie.MovieGenres.Where(mg => !newGenreNames.Contains(mg.Genre.Name)).ToList();
            var genreNamesToAdd = newGenreNames.Where(name => !currentGenreNames.Contains(name)).ToList();

            _context.MovieGenres.RemoveRange(genresToRemove);

            foreach (var genreName in genreNamesToAdd)
            {
                var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
                if (genre == null)
                {
                    genre = new Genre { Name = genreName };
                    _context.Genres.Add(genre);
                }
                movie.MovieGenres.Add(new MovieGenre { MovieId = movie.Id, Genre = genre });
            }

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            // Soft delete: set status to 'SoldOut'
            movie.Status = "SoldOut";
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
