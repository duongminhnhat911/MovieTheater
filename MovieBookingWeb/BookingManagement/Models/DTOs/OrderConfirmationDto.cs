namespace BookingManagement.Models.DTOs
{
    public class OrderConfirmationDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }
        public DateOnly BookingDate { get; set; }
        public int TotalPrice { get; set; }
        public required string ShowtimeDate { get; set; }
        public required string ShowtimeTime { get; set; }
        public required List<string> Seats { get; set; }
        public bool Status { get; set; }
    }
}
