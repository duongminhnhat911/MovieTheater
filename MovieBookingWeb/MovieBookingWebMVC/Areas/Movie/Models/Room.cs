namespace MovieBookingWebMVC.Areas.Movie.Models
{
    public class Room
    {
        public string? RoomName { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<Seat> Seats { get; set; } = new();
    }
}
