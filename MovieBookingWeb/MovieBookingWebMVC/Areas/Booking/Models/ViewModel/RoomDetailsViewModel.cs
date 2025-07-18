namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModel
{
    public class RoomDetailsViewModel
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = null!;
        public bool Status { get; set; }
        public int RoomQuantity { get; set; }
        public List<string> Seats { get; set; } = new();
    }
}
