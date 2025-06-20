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
        private const int PageSize = 10;
        private static List<Film> films;

        static AdminController()
        {
            films = MockFilmData.Films;
        }

        public AdminController(
            IWebHostEnvironment environment,
            IRoomService roomService,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor contextAccessor)
        {
            _environment = environment;
            _roomService = roomService;
            _httpClientFactory = httpClientFactory;
            _contextAccessor = contextAccessor;
        }

        private async Task<UserDto?> GetCurrentUserAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var cookie = _contextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
            if (!string.IsNullOrEmpty(cookie))
                client.DefaultRequestHeaders.Add("Cookie", cookie);

            var response = await client.GetAsync("api/user/profile");
            if (!response.IsSuccessStatusCode) return null;

            var userJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserDto>(userJson);
        }

        public IActionResult ViewMovie(int page = 1)
        {
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

            film.Id = films.Any() ? films.Max(f => f.Id) + 1 : 1;
            film.ShowtimesJson = FilmHelper.ConvertShowtimesToJson(film.Showtimes!);
            films.Add(film);

            TempData["Success"] = "Thêm phim thành công!";
            return RedirectToAction("ViewMovie");
        }

        [HttpGet]
        public IActionResult EditMovie(int id)
        {
            var film = films.FirstOrDefault(f => f.Id == id);
            if (film == null) return NotFound();

            ViewBag.AllRooms = _roomService.GetAllRoomLayouts().Keys.ToList();

            film.ShowtimesJson = FilmHelper.ConvertShowtimesToJson(film.Showtimes!);
            if (!string.IsNullOrEmpty(film.Rating)) film.RatingCode = film.Rating.Split(" - ")[0];
            return View(film);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMovie(Film model)
        {
            var film = films.FirstOrDefault(f => f.Id == model.Id);
            if (film == null) return NotFound();

            if (string.IsNullOrEmpty(film.Image) && (model.ImageFile == null || model.ImageFile.Length == 0))
                ModelState.AddModelError("ImageFile", "Vui lòng tải ảnh bìa");

            if (string.IsNullOrEmpty(film.CarouselImage) && (model.CarouselImageFile == null || model.CarouselImageFile.Length == 0))
                ModelState.AddModelError("CarouselImageFile", "Vui lòng tải ảnh carousel");

            film.Showtimes = FilmHelper.ParseShowtimesFromJson(model.ShowtimesJson ?? "[]");

            if (!ModelState.IsValid)
            {
                model.Showtimes = film.Showtimes;
                model.ShowtimesJson = JsonConvert.SerializeObject(film.Showtimes);
                ViewBag.AllRooms = _roomService.GetAllRoomLayouts().Keys.ToList();
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null) film.EditedByUsername = user.Username;

            film.Title = model.Title;
            film.TrailerLink = model.TrailerLink;
            film.Description = model.Description;
            film.Genres = model.Genres;
            film.Duration = model.Duration;
            film.Format = model.Format;
            film.ProductionCompany = model.ProductionCompany;
            film.RatingCode = model.RatingCode;

            if (!string.IsNullOrEmpty(film.RatingCode))
                film.Rating = FilmHelper.RatingHelper.GetFullRating(film.RatingCode);

            film.ReleaseDate = model.ReleaseDate;
            film.Director = model.Director;
            film.Cast = model.Cast;
            film.Subtitle = model.Subtitle;

            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
            await FilmHelper.SaveImagesAsync(model, uploadPath, useGuidName: true);

            if (model.ImageFile != null && model.ImageFile.Length > 0)
                film.Image = model.Image;

            if (model.CarouselImageFile != null && model.CarouselImageFile.Length > 0)
                film.CarouselImage = model.CarouselImage;

            film.ShowtimesJson = FilmHelper.ConvertShowtimesToJson(film.Showtimes);
            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction("EditMovie", new { id = film.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMovie(int id)
        {
            var film = films.FirstOrDefault(f => f.Id == id);
            if (film == null)
            {
                TempData["Error"] = "Không tìm thấy phim cần xoá.";
                return RedirectToAction("ViewMovie");
            }

            var wwwRoot = _environment.WebRootPath;

            if (!string.IsNullOrWhiteSpace(film.Image))
            {
                var imagePath = Path.Combine(wwwRoot, film.Image.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
            }

            if (!string.IsNullOrWhiteSpace(film.CarouselImage))
            {
                var carouselPath = Path.Combine(wwwRoot, film.CarouselImage.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(carouselPath)) System.IO.File.Delete(carouselPath);
            }

            films.Remove(film);
            TempData["Success"] = $"Đã xoá phim \"{film.Title}\" và ảnh liên quan thành công!";
            return RedirectToAction("ViewMovie");
        }

        [HttpGet]
        public async Task<IActionResult> ViewAccounts(int page = 1)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
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
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"api/user/{id}");

            TempData["Success"] = response.IsSuccessStatusCode
                ? "Đã xoá người dùng thành công."
                : "Không xoá được người dùng.";
            return RedirectToAction("ViewAccounts");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLockAccount(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PutAsync($"api/user/toggle-lock/{id}", null);

            TempData["Success"] = response.IsSuccessStatusCode
                ? "Đã đổi trạng thái khoá tài khoản."
                : "Không cập nhật được tài khoản.";
            return RedirectToAction("ViewAccounts");
        }
    }
}