namespace MovieBookingWeb.Models
{
    public class BookingViewModel
    {
        public string? Title { get; set; }
        public string? Image { get; set; }
        public List<string>? Genres { get; set; }
        public int? Duration { get; set; }
        public string? Rating { get; set; }
        public string? Subtitle { get; set; }
        public string? Director { get; set; }
        public string? Cast { get; set; }
        public List<string>? Format { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? Description { get; set; }
        public string? ProductionCompany { get; set; }
        public List<string>? AvailableDates { get; set; }
        public Dictionary<string, Dictionary<string, string>>? RoomByDateTime { get; set; }
        public Dictionary<string, Dictionary<string, List<string>>>? BookedSeats { get; set; } 
        public Dictionary<string, Room>? RoomLayouts { get; set; }
        public List<Showtime>? Showtimes { get; set; }
    }
    public class BookingShowtime
    {
        public DateTime Date { get; set; }
        public string? Time { get; set; }
        public string? RoomName { get; set; }
        public List<string>? BookedSeats { get; set; }
    }
}