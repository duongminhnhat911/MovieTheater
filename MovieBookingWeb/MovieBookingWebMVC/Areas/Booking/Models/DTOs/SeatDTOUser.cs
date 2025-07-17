namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class SeatDTOUser
    {
        public int SeatId { get; set; }
        public string Row { get; set; } = string.Empty;
        public string Column { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
