using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingWeb.Models;
using MovieBookingWeb.Services;
using MovieBookingWeb.Helper;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MovieBookingWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IRoomService _roomService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly MovieApiService _movieApiService;
        private const int PageSize = 10;

        public AdminController(
            IWebHostEnvironment environment,
            IRoomService roomService,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor contextAccessor,
            MovieApiService movieApiService)
        {
            _environment = environment;
            _roomService = roomService;
            _httpClientFactory = httpClientFactory;
            _contextAccessor = contextAccessor;
            _movieApiService = movieApiService;
        }

        private async Task<UserDto?> GetCurrentUserAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient_User");
            var cookie = _contextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
            if (!string.IsNullOrEmpty(cookie))
                client.DefaultRequestHeaders.Add("Cookie", cookie);

            var response = await client.GetAsync("api/user/profile");
            if (!response.IsSuccessStatusCode) return null;

            var userJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserDto>(userJson);
        }

        public async Task<IActionResult> ViewMovie(int page = 1)
        {
            try
            {
                var films = await _movieApiService.GetMovies();
                int totalFilms = films.Count;
                int totalPages = (int)Math.Ceiling((double)totalFilms / PageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                var pagedFilms = films
                    .OrderByDescending(f => f.ReleaseDate)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                return View(pagedFilms);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Lỗi API: {ex.Message}. Vui lòng đảm bảo API đang chạy.";
                return View(new List<Film>());
            }
        }

        [HttpGet]
        public IActionResult AddMovie()
        {
            var allRooms = _roomService.GetAllRoomLayouts().Keys.ToList();
            if (!allRooms.Any()) ModelState.AddModelError("", "Chưa có phòng chiếu nào trong hệ thống.");
            ViewBag.AllRooms = allRooms;

            var newFilm = new Film
            {
                Showtimes = new List<Showtime>(),
                ShowtimesJson = "[]"
            };

            return View(newFilm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMovie(Film film)
        {
            if (film.ImageFile == null || film.ImageFile.Length == 0)
                ModelState.AddModelError("ImageFile", "Vui lòng tải ảnh bìa");
            if (film.CarouselImageFile == null || film.CarouselImageFile.Length == 0)
                ModelState.AddModelError("CarouselImageFile", "Vui lòng tải ảnh carousel");

            film.Showtimes = FilmHelper.ParseShowtimesFromJson(film.ShowtimesJson ?? "[]");

            if (!ModelState.IsValid)
            {
                ViewBag.AllRooms = _roomService.GetAllRoomLayouts().Keys.ToList();
                film.ShowtimesJson = JsonConvert.SerializeObject(film.Showtimes);
                return View(film);
            }

            if (!string.IsNullOrEmpty(film.RatingCode) &&
                FilmHelper.RatingHelper.GetFullRating(film.RatingCode) is string fullRating)
            {
                film.Rating = fullRating;
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                film.CreatedByUsername = user.Username;
                film.CreatedByUserId = user.Id;
            }

            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
            await FilmHelper.SaveImagesAsync(film, uploadPath, useGuidName: true);

            film.ShowtimesJson = FilmHelper.ConvertShowtimesToJson(film.Showtimes!);
            
            try
            {
                var movieDto = new MovieCreateDto
                {
                    Title = film.Title,
                    Image = film.Image,
                    CarouselImage = film.CarouselImage,
                    TrailerLink = film.TrailerLink,
                    Subtitle = film.Subtitle,
                    RatingCode = film.RatingCode,
                    Duration = film.Duration,
                    Genres = film.Genres,
                    Director = film.Director,
                    Cast = film.Cast,
                    Description = film.Description,
                    ProductionCompany = film.ProductionCompany,
                    Format = film.Format,
                    ReleaseDate = film.ReleaseDate,
                    Showtimes = film.Showtimes.Select(s => new ShowtimeCreateDto { Date = s.Date, Time = s.Time.ToString(@"hh\:mm"), RoomName = s.RoomName }).ToList()
                };

                await _movieApiService.AddMovie(movieDto);

                TempData["Success"] = "Thêm phim thành công!";
                return RedirectToAction("ViewMovie");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Lỗi API: {ex.Message}. Vui lòng đảm bảo API đang chạy.");
                ViewBag.AllRooms = _roomService.GetAllRoomLayouts().Keys.ToList();
                return View(film);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditMovie(int id)
        {
            try
            {
                var film = await _movieApiService.GetMovie(id);
                if (film == null) return NotFound();

                ViewBag.AllRooms = _roomService.GetAllRoomLayouts().Keys.ToList();

                film.ShowtimesJson = FilmHelper.ConvertShowtimesToJson(film.Showtimes!);
                if (!string.IsNullOrEmpty(film.Rating)) film.RatingCode = film.Rating.Split(" - ")[0];
                return View(film);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Lỗi API: {ex.Message}. Vui lòng đảm bảo API đang chạy.";
                return RedirectToAction("ViewMovie");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMovie(Film model)
        {
            // First, parse showtimes from the JSON string to the model's list property
            model.Showtimes = FilmHelper.ParseShowtimesFromJson(model.ShowtimesJson ?? "[]");

            // Manually check for image files if the existing image properties are empty
            if (string.IsNullOrEmpty(model.Image) && (model.ImageFile == null || model.ImageFile.Length == 0))
                ModelState.AddModelError("ImageFile", "Vui lòng tải ảnh bìa");

            if (string.IsNullOrEmpty(model.CarouselImage) && (model.CarouselImageFile == null || model.CarouselImageFile.Length == 0))
                ModelState.AddModelError("CarouselImageFile", "Vui lòng tải ảnh carousel");
            
            if (!ModelState.IsValid)
            {
                ViewBag.AllRooms = _roomService.GetAllRoomLayouts().Keys.ToList();
                return View(model);
            }

            try
            {
                // Save uploaded images and update model properties
                string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                await FilmHelper.SaveImagesAsync(model, uploadPath, useGuidName: true);

                // Map the processed Film model to the DTO for the API call
                var movieDto = new MovieCreateDto
                {
                    Id = model.Id,
                    Title = model.Title,
                    Image = model.Image, // Use the updated image path
                    CarouselImage = model.CarouselImage, // Use the updated carousel image path
                    TrailerLink = model.TrailerLink,
                    Subtitle = model.Subtitle,
                    RatingCode = model.RatingCode,
                    Duration = model.Duration,
                    Genres = model.Genres,
                    Director = model.Director,
                    Cast = model.Cast,
                    Description = model.Description,
                    ProductionCompany = model.ProductionCompany,
                    Format = model.Format,
                    ReleaseDate = model.ReleaseDate,
                    Showtimes = model.Showtimes.Select(s => new ShowtimeCreateDto { Date = s.Date, Time = s.Time.ToString(@"hh\:mm"), RoomName = s.RoomName }).ToList()
                };

                await _movieApiService.UpdateMovie(movieDto);

                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction("EditMovie", new { id = model.Id });
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Lỗi API: {ex.Message}. Vui lòng đảm bảo API đang chạy.");
                ViewBag.AllRooms = _roomService.GetAllRoomLayouts().Keys.ToList();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                await _movieApiService.DeleteMovie(id);
                TempData["Success"] = "Xoá phim thành công!";
                return RedirectToAction("ViewMovie");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Lỗi API: {ex.Message}. Vui lòng đảm bảo API đang chạy.";
                return RedirectToAction("ViewMovie");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewAccounts(int page = 1)
        {
            var client = _httpClientFactory.CreateClient("ApiClient_User");
            var response = await client.GetAsync($"api/user?role=User&page={page}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Lỗi khi tải danh sách người dùng.";
                return RedirectToAction("ViewMovie");
            }

            var json = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<UserDto>>(json);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = 1; // Giả định nếu API không trả về tổng
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient_User");
            var response = await client.DeleteAsync($"api/user/{id}");

            TempData["Success"] = response.IsSuccessStatusCode
                ? "Đã xoá người dùng thành công."
                : "Không xoá được người dùng.";
            return RedirectToAction("ViewAccounts");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLockAccount(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient_User");
            var response = await client.PutAsync($"api/user/toggle-lock/{id}", null);

            TempData["Success"] = response.IsSuccessStatusCode
                ? "Đã đổi trạng thái khoá tài khoản."
                : "Không cập nhật được tài khoản.";
            return RedirectToAction("ViewAccounts");
        }
    }
}