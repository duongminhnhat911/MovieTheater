using MovieBookingWebMVC.Areas.Booking.Models.ViewModels;

namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModel
{
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }

        // Thông tin đơn hàng
        public int UserId { get; set; }
        public string BookingDate { get; set; } = "";
        public int TotalPrice { get; set; }
        public string Status { get; set; } = "";
        public string? PromotionCode { get; set; }

        // Thông tin suất chiếu
        public string ShowDate { get; set; } = "";
        public string FromTime { get; set; } = "";
        public string ToTime { get; set; } = "";

        // Thông tin phòng
        public string RoomName { get; set; } = "";

        // Danh sách ghế
        public List<string> Seats { get; set; } = new();

        // Tổng tiền từng ghế (nếu cần tách)
        public int TotalDetailPrice { get; set; }
    }
}
