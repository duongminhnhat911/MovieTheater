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
        private readonly MovieApiService _movieApiService;

        public BookingController(IRoomService roomService, MovieApiService movieApiService)
        {
            _roomService = roomService;
            _movieApiService = movieApiService;
        }

        public async Task<IActionResult> Index(int id)
        {
            try
            {
                var film = await _movieApiService.GetMovie(id);
                if (film == null) return NotFound();

                var viewModel = new BookingViewModel
                {
                    Title = film.Title,
                    Image = film.Image,
                    Genres = film.Genres,
                    Duration = film.Duration,
                    Rating = film.Rating,
                    Subtitle = film.Subtitle,
                    Director = film.Director,
                    Cast = film.Cast,
                    Format = film.Format,
                    ReleaseDate = film.ReleaseDate,
                    Description = film.Description,
                    ProductionCompany = film.ProductionCompany,
                    Showtimes = film.Showtimes,

                    AvailableDates = film.Showtimes?
        .Select(s => s.Date.ToString("yyyy-MM-dd"))
        .Distinct()
        .ToList(),

                    RoomByDateTime = film.Showtimes?
    .GroupBy(s => s.Date.ToString("yyyy-MM-dd"))
    .ToDictionary(
        g => g.Key,
        g => g.ToDictionary(
            s => s.Time.ToString(@"hh\:mm"),
            s => s.RoomName ?? ""
        )
    ),
                    BookedSeats = film.BookedSeats,
                    RoomLayouts = _roomService.GetAllRoomLayouts()
                };
                return View(viewModel);
            }
            catch (HttpRequestException ex)
            {
                // Log the exception
                TempData["Error"] = "Không thể tải thông tin chi tiết phim. Vui lòng thử lại sau.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult Confirm(string SelectedSeats)
        {
            return Content($"Bạn đã đặt các ghế: {SelectedSeats}");
        }
    }
}