namespace MovieBookingWeb.Models
{
    public class Film
    {
        public string? Title { get; set; }
        public string? Tagline { get; set; }
        public string? Image { get; set; }
        public string? CarouselImage { get; set; }
        public string? TrailerLink { get; set; }
        public string? BookingLink { get; set; }
        public string? Subtitle { get; set; }
        public string? Rating { get; set; }
        public string? Duration { get; set; }
        public string? Genre { get; set; }
        public string? Director { get; set; }
        public string? Cast { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? Description { get; set; }
        public List<Showtime>? Showtimes { get; set; }
        public List<string>? AvailableDates { get; set; }
        public Dictionary<string, Dictionary<string, string>>? RoomByDateTime { get; set; }
        public Dictionary<string, Dictionary<string, List<string>>>? BookedSeats { get; set; }
    }
}
