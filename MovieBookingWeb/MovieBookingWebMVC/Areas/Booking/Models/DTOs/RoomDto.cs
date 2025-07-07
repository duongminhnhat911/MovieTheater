namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = null!;
        public int RoomQuantity { get; set; }
        public bool Status { get; set; }
    }
}
