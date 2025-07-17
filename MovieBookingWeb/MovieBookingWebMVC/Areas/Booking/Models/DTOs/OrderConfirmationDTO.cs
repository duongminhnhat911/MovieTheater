namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class OrderConfirmationDTO
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }
        public DateOnly BookingDate { get; set; }
        public int TotalPrice { get; set; }
        public string ShowtimeDate { get; set; } = string.Empty;
        public string ShowtimeTime { get; set; } = string.Empty;
        public List<string> Seats { get; set; } = new();
        public bool Status { get; set; }
    }
}
