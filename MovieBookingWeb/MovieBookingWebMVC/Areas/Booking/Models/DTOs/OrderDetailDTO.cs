namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }

        public OrderDTO Order { get; set; } = new();
        public ShowtimeDTO Showtime { get; set; } = new();
        public SeatDTO Seat { get; set; } = new();
    }
}
