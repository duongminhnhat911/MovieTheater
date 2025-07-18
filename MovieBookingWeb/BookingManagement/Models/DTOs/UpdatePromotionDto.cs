namespace BookingManagement.Models.DTOs
{
    public class UpdatePromotionDto
    {
        public string PromotionCode { get; set; } = string.Empty;
        public string? Description { get; set; }

        public int? DiscountAmount { get; set; }
        public int? DiscountPercent { get; set; }

        public int Quantity { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
