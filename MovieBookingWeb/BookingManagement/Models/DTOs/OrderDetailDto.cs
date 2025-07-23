namespace BookingManagement.Models.DTOs
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string BookingDate { get; set; } = "";
        public int TotalPrice { get; set; }
        public string Status { get; set; } = "";
        public string PromotionCode { get; set; } = "";

        public int MovieId { get; set; }
        public string ShowDate { get; set; } = "";
        public string FromTime { get; set; } = "";
        public string ToTime { get; set; } = "";
        public string RoomName { get; set; } = "";

        public List<string> Seats { get; set; } = new();
        public int TotalDetailPrice { get; set; }

    }
}
