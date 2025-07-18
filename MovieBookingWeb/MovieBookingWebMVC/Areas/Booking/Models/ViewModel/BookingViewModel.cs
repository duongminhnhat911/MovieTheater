namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModel
{
    public class BookingViewModel
    {
        public int MovieId { get; set; }
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
    }
    
}
