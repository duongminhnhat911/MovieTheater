namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class ShowtimeDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string Room { get; set; } = string.Empty;

        public DateOnly ShowDate { get; set; }
        public TimeOnly FromTime { get; set; }
        public TimeOnly ToTime { get; set; }
    }
}
