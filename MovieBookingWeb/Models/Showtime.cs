namespace MovieBookingWeb.Models
{
    public class Showtime
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string? RoomName { get; set; }
    }
}