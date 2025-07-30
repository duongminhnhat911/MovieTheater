namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModel
{
    public class SeatViewModel
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public RoomDetailsViewModel Room { get; set; } = new();
        public string SeatRow { get; set; } = "";
        public string SeatColumn { get; set; } = "";
        public bool SeatStatus { get; set; }

    }
}
