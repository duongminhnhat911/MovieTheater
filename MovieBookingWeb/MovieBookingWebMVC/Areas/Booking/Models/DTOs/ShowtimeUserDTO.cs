namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class ShowtimeUserDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }

        public DateOnly ShowDate { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public int AvailableSeats { get; set; }

        // Các thông tin thêm về phim
        public string MovieTitle { get; set; } = string.Empty;
        public string MoviePoster { get; set; } = string.Empty;
        public int? Duration { get; set; }
        public List<string> Format { get; set; } = new();
        public List<string> Genres { get; set; } = new();

        public string RoomName { get; set; } = string.Empty;
    }
}
