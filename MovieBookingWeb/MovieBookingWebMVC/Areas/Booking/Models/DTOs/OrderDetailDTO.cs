namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }

        public OrderDTO Order { get; set; } = new();
        public ShowtimeDto Showtime { get; set; } = new();
        public SelectedSeatDTO Seat { get; set; } = new();
    }
}
