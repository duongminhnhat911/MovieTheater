namespace BookingManagement.Models.DTOs
{
    public class UpdateOrderDetailDto
    {
        public int? SeatId { get; set; }
        public int? ShowtimeId { get; set; }
        public int? Price { get; set; }
    }
}
