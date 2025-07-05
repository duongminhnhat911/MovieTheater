namespace MovieBookingWebMVC.Areas.Movie.Models.ViewModel
{
    public class ShowtimeViewModel
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string? RoomName { get; set; }
    }
}
