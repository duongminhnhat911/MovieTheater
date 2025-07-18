using System.ComponentModel.DataAnnotations;

namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModel
{
    public class PromotionListViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Mã khuyến mãi là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã khuyến mãi không được quá 50 ký tự")]
        public string PromotionCode { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Mô tả không được quá 200 ký tự")]
        public string? Description { get; set; }
        
        [Range(0, 1000000, ErrorMessage = "Số tiền giảm phải từ 0 đến 1,000,000")]
        public int? DiscountAmount { get; set; }
        
        [Range(0, 100, ErrorMessage = "Phần trăm giảm phải từ 0 đến 100")]
        public int? DiscountPercent { get; set; }
        
        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, 10000, ErrorMessage = "Số lượng phải từ 1 đến 10,000")]
        public int Quantity { get; set; }
        
        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }
        
        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; }
    }
}
