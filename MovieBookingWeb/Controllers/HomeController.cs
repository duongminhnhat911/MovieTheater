using Microsoft.AspNetCore.Mvc;
using MovieBookingWeb.Models;
using MovieBookingWeb.Services;

namespace MovieBookingWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieApiService _movieApiService;

        public HomeController(MovieApiService movieApiService)
        {
            _movieApiService = movieApiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var films = await _movieApiService.GetMovies();
                var carousel = films.Where(f => !string.IsNullOrEmpty(f.CarouselImage)).ToList();
                var nowShowing = films.Where(f => f.ReleaseDate <= DateTime.Now).ToList();
                var comingSoon = films.Where(f => f.ReleaseDate > DateTime.Now).ToList();

                var model = new HomeViewModel
                {
                    Carousel = carousel,
                    NowShowing = nowShowing,
                    ComingSoon = comingSoon
                };
                return View(model);
            }
            catch (HttpRequestException ex)
            {
                // Log the exception: _logger.LogError(ex, "API call failed.");
                ViewData["Error"] = "Không thể tải dữ liệu phim lúc này. Vui lòng thử lại sau.";
                return View(new HomeViewModel());
            }
        }
    }
}