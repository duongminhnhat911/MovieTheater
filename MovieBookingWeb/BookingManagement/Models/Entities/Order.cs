namespace BookingManagement.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateOnly BookingDate { get; set; }

        public int TotalPrice { get; set; }

        public required bool Status { get; set; }
        public int? PromotionId { get; set; }
        public Promotion? Promotion { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}
