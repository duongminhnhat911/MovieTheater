using MovieBookingWebMVC.Areas.Movie.Models.ViewModel;
namespace MovieBookingWebMVC.Models
{
    public class HomeViewModel
    {
        public List<FilmViewModel>? Carousel { get; set; } = new List<FilmViewModel>();
        public List<FilmViewModel>? NowShowing { get; set; } = new List<FilmViewModel>();
        public List<FilmViewModel>? ComingSoon { get; set; } = new List<FilmViewModel>();
    }
}
