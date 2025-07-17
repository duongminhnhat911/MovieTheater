namespace BookingManagement.Models.DTOs
{
    public class CreateOrderDetailDto
    {
        public int OrderId { get; set; }
        public int SeatId { get; set; }
        public int ShowtimeId { get; set; }
        public int Price { get; set; }
    }
}
