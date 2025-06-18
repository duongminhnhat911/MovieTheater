namespace MovieBookingWeb.Models
{
    public class BookingViewModel
    {
        public string? Title { get; set; }
        public string? Image { get; set; }
        public string? Genre { get; set; }
        public string? Duration { get; set; }
        public string? Rating { get; set; }
        public string? Subtitle { get; set; }
        public string? Director { get; set; }
        public string? Cast { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? Description { get; set; }
        public List<string>? AvailableDates { get; set; }
        public Dictionary<string, Dictionary<string, string>> RoomByDateTime { get; set; } = new();
        public Dictionary<string, Dictionary<string, List<string>>> BookedSeats { get; set; } = new();
        public Dictionary<string, Room> RoomLayouts { get; set; } = new();
        public List<Showtime>? Showtimes { get; set; }
    }
    public class Showtime
    {
        public DateTime Date { get; set; }
        public string Time { get; set; } = "";
        public string RoomName { get; set; } = "";
        public List<string> BookedSeats { get; set; } = new();
    }
}