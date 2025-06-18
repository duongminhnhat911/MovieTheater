using Microsoft.AspNetCore.Mvc;
using MovieBookingWeb.Models;

namespace MovieBookingWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var carousel = new List<Film>
            {
                new Film {
                    Title = "Dune: Hành tinh cát",
                    Tagline = "Sử thi khoa học viễn tưởng của năm",
                    Image = "/images/dune.jpg",
                    CarouselImage = "/images/dune-carousel.jpg",
                    TrailerLink = "https://www.youtube.com/watch?v=R797Sf-2zt0",
                    BookingLink = "/booking/dune" },
                new Film {
                    Title = "Godzilla x Kong",
                    Tagline = "Cuộc đối đầu thế kỷ",
                    Image = "/images/godzilla-vs-kong.jpg",
                    CarouselImage = "/images/godzillavskong-carousel.jpg",
                    TrailerLink = "/trailer/godzilla-vs-kong",
                    BookingLink = "/booking/godzilla-vs-kong" },
                new Film {
                    Title = "Inside Out 2",
                    Tagline = "Hành trình cảm xúc tuổi teen",
                    Image = "/images/insideout2.jpg",
                    CarouselImage = "/images/carousel-io2.jpg",
                    TrailerLink = "https://www.youtube.com/watch?v=AfOlW2OrzqE", 
                    BookingLink = "/booking/inside-out-2" },
            };

            var nowShowing = new List<Film>(carousel);

            var comingSoon = new List<Film>
            {
                new Film { Title = "Deadpool & Wolverine", Tagline = "Màn kết hợp đỉnh cao", Image = "/images/deadpool-wolverine.jpg", TrailerLink = "/trailer/deadpool-wolverine", BookingLink = "/booking/deadpool-wolverine" },
                new Film { Title = "Avatar 3", Tagline = "Hành trình tiếp theo tại Pandora", Image = "/images/avatar3.jpg", TrailerLink = "/trailer/avatar3", BookingLink = "/booking/avatar3" },
            };

            var vm = new HomeViewModel
            {
                Carousel = carousel,
                NowShowing = nowShowing,
                ComingSoon = comingSoon
            };

            return View(vm);
        }
    }
}