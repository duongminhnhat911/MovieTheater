namespace MovieBookingWeb.Models
{
    public class HomeViewModel
    {
        //public List<Film>? Carousel { get; set; }
        //public List<Film>? NowShowing { get; set; }
        //public List<Film>? ComingSoon { get; set; }
        public List<Film>? Carousel { get; set; } = new List<Film>();
        public List<Film>? NowShowing { get; set; } = new List<Film>();
        public List<Film>? ComingSoon { get; set; } = new List<Film>();
    }
}