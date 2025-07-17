namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class ShowtimeShortDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public DateTime ShowDate { get; set; }
        public string FromTime { get; set; } = string.Empty;
        public string ToTime { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
    }

    public class MovieWithShowtimeDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int Duration { get; set; }
        public List<string> Genres { get; set; } = new();
        public List<string> Format { get; set; } = new();
        public string Director { get; set; } = string.Empty;
        public List<ShowtimeShortDTO> Showtimes { get; set; } = new();
    }
}
