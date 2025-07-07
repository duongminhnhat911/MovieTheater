using System.Text.Json.Serialization;

namespace MovieBookingWebMVC.Areas.Booking.Models
{
    public class ShowTime
    {
        public int Id { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        [JsonPropertyName("room")]  // 🔧 GIẢI QUYẾT LỖI KHÔNG SHOW RA PHÒNG
        public string RoomName { get; set; } = string.Empty;

        // Movie Info
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string MoviePoster { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = new();
        public List<string> Format { get; set; } = new();
        public int? Duration { get; set; }
    }
}
