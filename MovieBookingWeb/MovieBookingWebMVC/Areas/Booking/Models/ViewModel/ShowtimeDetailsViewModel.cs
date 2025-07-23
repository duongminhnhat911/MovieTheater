namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModel
{
    public class ShowtimeDetailsViewModel
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public RoomDetailsViewModel Room { get; set; } = new();
        public DateOnly ShowDate { get; set; }
        public TimeOnly FromTime { get; set; }
        public TimeOnly ToTime { get; set; }

    }
}
