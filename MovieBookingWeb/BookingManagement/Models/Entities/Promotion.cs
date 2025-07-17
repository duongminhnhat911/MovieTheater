using System.ComponentModel.DataAnnotations;

namespace BookingManagement.Models.Entities
{
    public class Promotion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string PromotionCode { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public int? DiscountAmount { get; set; }   // Giảm theo số tiền

        public int? DiscountPercent { get; set; }  // Giảm theo %

        public int Quantity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
