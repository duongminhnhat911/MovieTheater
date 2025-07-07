using System.Text.Json.Serialization;

namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class ShowtimeDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public int AvailableSeats { get; set; }

        public string MovieTitle { get; set; } = string.Empty;
        public string MoviePoster { get; set; } = string.Empty;
        public int? Duration { get; set; }
        public List<string> Format { get; set; } = new();
        public List<string> Genres { get; set; } = new();

        [JsonPropertyName("room")]
        public RoomDto? Room { get; set; }

        [JsonIgnore]
        public string RoomName => Room?.RoomName ?? string.Empty;
    }
   
}
