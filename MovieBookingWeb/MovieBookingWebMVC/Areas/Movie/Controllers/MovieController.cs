using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingWebMVC.Areas.Movie.Helper;
using MovieBookingWebMVC.Areas.Movie.Models.DTOs;
using MovieBookingWebMVC.Areas.Movie.Services;
using Newtonsoft.Json;
using MovieBookingWebMVC.Areas.User.Models.DTOs;
using MovieBookingWebMVC.Areas.Movie.Models.ViewModel;

namespace MovieBookingWebMVC.Areas.Movie.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Movie")]
    public class MovieController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly MovieApiService _movieApiService;
        private const int PageSize = 5;

        public MovieController(
            IWebHostEnvironment environment,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor contextAccessor,
            MovieApiService movieApiService)
        {
            _environment = environment;
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
                return View(new List<FilmViewModel>());
            }
        }

        [HttpGet]
        public IActionResult AddMovie()
        {
            return View(new FilmViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMovie(FilmViewModel film)
        {
            if (film.ImageFile == null || film.ImageFile.Length == 0)
                ModelState.AddModelError("ImageFile", "Vui lòng tải ảnh bìa");
            if (film.CarouselImageFile == null || film.CarouselImageFile.Length == 0)
                ModelState.AddModelError("CarouselImageFile", "Vui lòng tải ảnh carousel");

            if (!ModelState.IsValid)
            {
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

            try
            {
                film.Actors = film.CastText?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
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
                    Actors = film.Actors,
                    Director = film.Director,
                    Description = film.Description,
                    ProductionCompany = film.ProductionCompany,
                    Format = film.Format,
                    ReleaseDate = film.ReleaseDate,
                    CreatedByUsername = film.CreatedByUsername,
                    Status = film.Status
                };

                await _movieApiService.AddMovie(movieDto);

                TempData["Success"] = "Thêm phim thành công!";
                return RedirectToAction("ViewMovie", "Movie", new { area = "Movie", page = Request.Query["page"] });
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Lỗi API: {ex.Message}. Vui lòng đảm bảo API đang chạy.");
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

                // Xử lý diễn viên: nếu 1 phần tử nhưng có dấu xuống dòng → tách dòng
                if (film.Actors?.Count == 1 && film.Actors[0].Contains("\n"))
                {
                    film.Actors = film.Actors[0]
                        .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                film.CastText = string.Join(Environment.NewLine, film.Actors ?? new List<string>());

                if (!string.IsNullOrEmpty(film.Rating))
                    film.RatingCode = film.Rating.Split(" - ")[0];

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
        public async Task<IActionResult> EditMovie([FromForm] FilmViewModel model)
        {
            model.EditedByUsername = User.Identity?.Name ?? "unknown";

            if (string.IsNullOrEmpty(model.Image) && (model.ImageFile == null || model.ImageFile.Length == 0))
                ModelState.AddModelError("ImageFile", "Vui lòng tải ảnh bìa");

            if (string.IsNullOrEmpty(model.CarouselImage) && (model.CarouselImageFile == null || model.CarouselImageFile.Length == 0))
                ModelState.AddModelError("CarouselImageFile", "Vui lòng tải ảnh carousel");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                await FilmHelper.SaveImagesAsync(model, uploadPath, useGuidName: true);

                // Chuyển CastText thành danh sách diễn viên
                model.Actors = model.CastText?
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList() ?? new List<string>();

                var movieDto = new MovieCreateDto
                {
                    Id = model.Id,
                    Title = model.Title,
                    Image = model.Image,
                    CarouselImage = model.CarouselImage,
                    TrailerLink = model.TrailerLink,
                    Subtitle = model.Subtitle,
                    RatingCode = model.RatingCode,
                    Duration = model.Duration,
                    Genres = model.Genres,
                    Actors = model.Actors,
                    Director = model.Director,
                    Description = model.Description,
                    ProductionCompany = model.ProductionCompany,
                    Format = model.Format,
                    ReleaseDate = model.ReleaseDate,
                    EditedByUsername = model.EditedByUsername,
                    Status = model.Status
                };

                await _movieApiService.UpdateMovie(movieDto);

                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction("EditMovie", "Movie", new { area = "Movie", id = model.Id, page = Request.Query["page"] });
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Lỗi API: {ex.Message}. Vui lòng đảm bảo API đang chạy.");
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
                return RedirectToAction("ViewMovie", "Movie", new { area = "Movie", page = Request.Query["page"] });
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Lỗi API: {ex.Message}. Vui lòng đảm bảo API đang chạy.";
                return RedirectToAction("ViewMovie", "Movie", new { area = "Movie", page = Request.Query["page"] });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMovieDetail(int id)
        {
            var film = await _movieApiService.GetMovie(id); // Gọi API để lấy MovieCreateDto
            if (film == null) return NotFound();
            return Json(film);
        }
    }
}