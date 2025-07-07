namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }

        public OrderDTO Order { get; set; } = new();
        public ShowtimeDTOUser Showtime { get; set; } = new();
        public SeatDTOUser Seat { get; set; } = new();
    }
}
