namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class CreatePaymentRequestDto
    {
        public int ShowtimeId { get; set; }
        public int UserId { get; set; }
        public List<int> SeatIds { get; set; } = new();
        public string? PromotionCode { get; set; }
    }
}
