namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModel
{
    public class OrderSummaryViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool Status { get; set; }
        public int? PromotionId { get; set; }
        public object? Promotion { get; set; }
    }
}
