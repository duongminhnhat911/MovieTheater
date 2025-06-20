using Microsoft.AspNetCore.Mvc;
using MovieBookingWeb.Models;

namespace MovieBookingWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var carousel = MockFilmData.Films.Where(f => !string.IsNullOrEmpty(f.CarouselImage)).ToList();
            var nowShowing = MockFilmData.Films.ToList();     
            var comingSoon = MockFilmData.Films.ToList();
            var viewModel = new HomeViewModel
            {
                Carousel = carousel,
                NowShowing = nowShowing,
                ComingSoon = comingSoon
            };

            return View(viewModel);
        }
    }
}