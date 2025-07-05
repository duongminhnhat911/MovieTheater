namespace BookingManagement.Models.DTOs
{
    public class CreatePaymentRequestDto
    {
        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; } = new();
    }
}
