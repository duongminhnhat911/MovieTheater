using MovieBookingWebMVC.Areas.Booking.Models.DTOs;

namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModels
{
    public class ConfirmBookingViewModel
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string username { get; set; }
        public DateOnly BookingDate { get; set; }
        public int TotalPrice { get; set; }
        public bool Status { get; set; }
        public string RoomName { get; set; }

        public MovieDTO Movie { get; set; } = new();
        public ShowtimeDto Showtime { get; set; } = new();
        public List<SelectedSeatDTO> SelectedSeats { get; set; } = new();
    }
}
