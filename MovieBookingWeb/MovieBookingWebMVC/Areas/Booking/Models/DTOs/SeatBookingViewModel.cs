using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using MovieBookingWebMVC.Areas.Movie.Models.DTOs;
using MovieBookingWebMVC.Areas.Movie.Models.ViewModel;

namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModels
{
    public class SeatBookingViewModel
    {
        public int ShowtimeId { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeSpan FromTime { get; set; }
        public string RoomName { get; set; }

        public OrderDTO? Order { get; set; }

        // Dùng FilmViewModel thay vì MovieCreateDto
        public FilmViewModel Movie { get; set; }

        public List<SeatDTOUser> Seats { get; set; } = new();
    }
}
