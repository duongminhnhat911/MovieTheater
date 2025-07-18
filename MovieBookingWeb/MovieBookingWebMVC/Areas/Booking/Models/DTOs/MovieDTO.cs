namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
    }
}