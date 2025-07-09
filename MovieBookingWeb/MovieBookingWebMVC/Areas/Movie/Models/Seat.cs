namespace MovieBookingWebMVC.Areas.Movie.Models
{
    public class Seat
    {
        public string? SeatId { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsVIP { get; set; } = false;
        public bool IsBroken { get; set; } = false;
    }
}
