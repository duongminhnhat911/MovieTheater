namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class CreateShowtimeDto
    {
        public int MovieId { get; set; }
        public int RoomId { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly StartTime { get; set; }
    }
}
