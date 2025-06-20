using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingWeb.Models;
using MovieBookingWeb.Services;

namespace MovieBookingWeb.Controllers
{
    [Authorize(Roles = "User")]
    public class BookingController : Controller
    {
        private readonly IRoomService _roomService;

        public BookingController(IRoomService roomService)
        {
            _roomService = roomService;
        }
        public IActionResult Index(string id)
        {
            var film = MockFilmData.Films.FirstOrDefault(f => f.BookingLink != null && f.BookingLink.EndsWith("/" + id));
            if (film == null)
            {
                return NotFound($"Không tìm thấy phim với id: {id}");
            }
            var viewModel = new BookingViewModel
            {
                Title = film.Title,
                Image = film.Image,
                Genres = film.Genres,
                Duration = film.Duration ?? 0,
                Rating = film.Rating,
                Subtitle = film.Subtitle,
                ProductionCompany = film.ProductionCompany,
                Director = film.Director,
                Cast = film.Cast,
                Format = film.Format,
                ReleaseDate = film.ReleaseDate,
                Description = film.Description,
                Showtimes = film.Showtimes ?? new List<Showtime>(),
                RoomByDateTime = film.RoomByDateTime ?? new Dictionary<string, Dictionary<string, string>>(),
                BookedSeats = film.BookedSeats ?? new Dictionary<string, Dictionary<string, List<string>>>(),
                RoomLayouts = _roomService.GetAllRoomLayouts(),
                AvailableDates = (film.Showtimes ?? new List<Showtime>())
                    .Select(s => s.Date.ToString("yyyy-MM-dd"))
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Confirm(string SelectedSeats)
        {
            return Content($"Bạn đã đặt các ghế: {SelectedSeats}");
        }
    }
}