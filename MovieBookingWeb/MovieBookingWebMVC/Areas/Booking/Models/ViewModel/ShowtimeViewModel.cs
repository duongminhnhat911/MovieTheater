namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModel
{
        public class ShowtimeViewModel
        {
            public int Id { get; set; }
            public int MovieId { get; set; }
            public string MovieName { get; set; } = string.Empty;
            public string Room { get; set; } = string.Empty;
            public DateOnly ShowDate { get; set; }
            public TimeOnly FromTime { get; set; }
            public TimeOnly ToTime { get; set; }
        }
    }
