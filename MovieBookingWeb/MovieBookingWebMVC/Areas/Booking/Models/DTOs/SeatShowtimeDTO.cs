namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class SeatShowtimeDTO
    {
        public int RoomId { get; set; }
        public int ShowtimeId { get; set; }
        public List<SeatDTO> Seats { get; set; } = new();
    }
}
