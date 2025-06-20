namespace MovieBookingWeb.Models
{
    public class HomeViewModel
    {
        public List<Film>? Carousel { get; set; }
        public List<Film>? NowShowing { get; set; }
        public List<Film>? ComingSoon { get; set; }
    }
}