namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class RoomDetailsDto
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = null!;
        public int RoomQuantity { get; set; }
        public bool Status { get; set; }
        public List<string> Seats { get; set; } = new();
    }
}
