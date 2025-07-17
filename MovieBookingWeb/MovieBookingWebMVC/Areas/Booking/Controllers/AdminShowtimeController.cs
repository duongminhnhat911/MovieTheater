using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;
using MovieBookingWebMVC.Areas.Booking.Services;
using MovieBookingWebMVC.Areas.Movie.Services;
using IRoomService = MovieBookingWebMVC.Areas.Booking.Services.IRoomService;

namespace MovieBookingWebMVC.Areas.Booking.Controllers
{
    [Area("Booking")]
    public class AdminShowtimeController : Controller
    {
        private readonly IShowtimeApiService _showtimeService;
        private readonly MovieApiService _movieApiService;
        private readonly IRoomService _roomService;

        public AdminShowtimeController(
            IShowtimeApiService showtimeService,
            MovieApiService movieApiService,
            IRoomService roomService)
        {
            _showtimeService = showtimeService;
            _movieApiService = movieApiService;
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var showtimeDtos = await _showtimeService.GetAllShowtimesAsync();
            var movies = await _movieApiService.GetMovies();
            var rooms = await _roomService.GetRoomsAsync();

            var vm = showtimeDtos.Select(s => new ShowtimeViewModel
            {
                Id = s.Id,
                MovieId = s.MovieId,
                MovieName = movies.FirstOrDefault(m => m.Id == s.MovieId)?.Title ?? "N/A",
                Room = rooms.FirstOrDefault(r => r.RoomName == s.Room)?.RoomName ?? "N/A",
                ShowDate = s.ShowDate,
                FromTime = s.FromTime,
                ToTime = s.ToTime
            }).ToList();

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadMoviesAndRoomsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateShowtimeDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Thông tin chưa hợp lệ";
                await LoadMoviesAndRoomsAsync(); // Nạp lại dropdown
                return View(dto);
            }

            var result = await _showtimeService.CreateShowtimeAsync(dto);
            if (result)
            {
                TempData["Success"] = "✅ Tạo suất chiếu thành công!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "❌ Tạo suất chiếu thất bại.";
            await LoadMoviesAndRoomsAsync(); // Nạp lại nếu tạo thất bại
            return View(dto);
        }

        /// <summary>
        /// Nạp ViewBag.Movies và ViewBag.Rooms từ API
        /// </summary>
        private async Task LoadMoviesAndRoomsAsync()
        {
            var movies = await _movieApiService.GetMovies();
            var rooms = await _roomService.GetRoomsAsync();

            Console.WriteLine("🎥 Danh sách phim:");
            foreach (var m in movies)
                Console.WriteLine($"- {m.Id} | {m.Title}");

            Console.WriteLine("🏠 Danh sách phòng:");
            foreach (var r in rooms)
                Console.WriteLine($"- {r.Id} | {r.RoomName}");

            ViewBag.Movies = movies.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            ViewBag.Rooms = rooms.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.RoomName
            }).ToList();
        }
    }
}
