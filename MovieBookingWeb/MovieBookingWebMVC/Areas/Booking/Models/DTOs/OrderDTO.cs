namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateOnly BookingDate { get; set; }
        public int TotalPrice { get; set; }
        public bool Status { get; set; }
        //
        public int? PromotionId { get; set; }
    }
}
