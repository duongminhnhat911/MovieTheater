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
                .Include(m => m.Showtimes)
                .ToListAsync();

            var movieDtos = movies.Select(m => new FilmDto
            {
                Id = m.Id,
                Title = m.Title,
                Image = m.Image,
                CarouselImage = m.CarouselImage,
                TrailerLink = m.TrailerLink,
                Subtitle = m.Subtitle,
                RatingCode = m.RatingCode,
                Duration = m.Duration,
                Genres = m.MovieGenres?.Select(g => g.Genre?.Name).Where(name => name != null).ToList(),
                Director = m.Director,
                Cast = m.Cast,
                Description = m.Description,
                ProductionCompany = m.ProductionCompany,
                Format = m.Format,
                ReleaseDate = m.ReleaseDate,
                CreatedByUsername = m.CreatedByUsername,
                EditedByUsername = m.EditedByUsername,
                Showtimes = m.Showtimes?.Select(s => new ShowtimeDto
                {
                    Date = s.Date,
                    Time = s.Time,
                    RoomName = s.RoomName
                }).ToList()
            }).ToList();

            return Ok(movieDtos);
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
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
                CreatedByUsername = filmDto.CreatedByUsername ?? "Unknown", // ✅ LẤY TỪ DTO
                EditedByUsername = filmDto.CreatedByUsername ?? "Unknown",
                Showtimes = filmDto.Showtimes?.Select(s => new Showtime
                {
                    Date = s.Date.ToUniversalTime(),
                    Time = s.Time,
                    RoomName = s.RoomName
                }).ToList() ?? new List<Showtime>(),
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
        public async Task<IActionResult> PutMovie(int id, [FromBody] FilmUpdateDto updateDto)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieGenres)
                .Include(m => m.Showtimes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            // Cập nhật thông tin cơ bản
            movie.Title = updateDto.Title;
            movie.Image = updateDto.Image;
            movie.CarouselImage = updateDto.CarouselImage;
            movie.TrailerLink = updateDto.TrailerLink;
            movie.Subtitle = updateDto.Subtitle;
            movie.RatingCode = updateDto.RatingCode;
            movie.Duration = updateDto.Duration;
            movie.Director = updateDto.Director;
            movie.Cast = updateDto.Cast;
            movie.Description = updateDto.Description;
            movie.ProductionCompany = updateDto.ProductionCompany;
            movie.Format = updateDto.Format;
            movie.ReleaseDate = (updateDto.ReleaseDate ?? DateTime.Now).ToUniversalTime();

            // ✅ Ghi lại người chỉnh sửa từ session thực
            movie.EditedByUsername = User?.Identity?.Name ?? "Unknown";
            // movie.UpdatedAt = DateTime.UtcNow; // nếu bạn có field này

            // Cập nhật genres
            movie.MovieGenres.Clear();
            if (updateDto.Genres?.Any() == true)
            {
                foreach (var genreName in updateDto.Genres)
                {
                    var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreName)
                                ?? new Genre { Name = genreName };
                    movie.MovieGenres.Add(new MovieGenre { Genre = genre });
                }
            }

            // Cập nhật showtimes
            movie.Showtimes.Clear();
            if (updateDto.Showtimes?.Any() == true)
            {
                foreach (var s in updateDto.Showtimes)
                {
                    movie.Showtimes.Add(new Showtime
                    {
                        Date = s.Date.ToUniversalTime(),
                        Time = s.Time,
                        RoomName = s.RoomName
                    });
                }
            }
            Console.WriteLine($"[DEBUG] User: {User.Identity?.Name}, Authenticated: {User.Identity?.IsAuthenticated}");

            await _context.SaveChangesAsync();
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

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
} 