namespace MovieBookingWebMVC.Areas.Booking.Models
{
    public class ShowtimeViewModel
    {
        public DateTime Date { get; set; }
        public List<ShowtimeEntry> Showtimes { get; set; }
    }

    public class ShowtimeEntry
    {
        public int ShowtimeId { get; set; }
        public TimeSpan Time { get; set; }
        public int AvailableSeats { get; set; } 
    }
}
