namespace BookingManagement.Models.DTOs
{
    public class OrderHistoryDto
    {
        public int OrderId { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly FromTime { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public List<string> Seats { get; set; } = new();
        public int TotalPrice { get; set; }
        public bool Status { get; set; }
        public DateOnly BookingDate { get; set; }

    }
}
