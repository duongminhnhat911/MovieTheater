using System.ComponentModel.DataAnnotations;

namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModel
{
    public class CreatePromotionViewModel
    {
        [Required]
        [Display(Name = "Mã khuyến mãi")]
        public string PromotionCode { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Giảm theo số tiền")]
        public int? DiscountAmount { get; set; }

        [Display(Name = "Giảm theo phần trăm")]
        public int? DiscountPercent { get; set; }

        [Required]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}
