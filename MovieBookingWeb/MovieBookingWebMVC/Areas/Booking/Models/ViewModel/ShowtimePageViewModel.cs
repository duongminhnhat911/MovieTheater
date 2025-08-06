using MovieBookingWebMVC.Areas.Booking.Models.DTOs;

namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModel
{
    public class ShowtimePageViewModel
    {
        public MovieDTO Movie { get; set; } = new();
        public List<ShowtimeDTOUser> Showtimes { get; set; } = new();
    }

}
