namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class SeatDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public char SeatRow { get; set; }
        public char SeatColumn { get; set; }
        public bool SeatStatus { get; set; }
    }
}
