namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class SeatShowtimeDTO
    {
        public int RoomId { get; set; }
        public int ShowtimeId { get; set; }
        public List<SeatDTOUser> Seats { get; set; } = new();
    }
}
