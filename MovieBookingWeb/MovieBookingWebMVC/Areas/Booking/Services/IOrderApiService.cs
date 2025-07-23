using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public interface IOrderApiService
    {
        Task<List<OrderSummaryViewModel>> GetAllOrdersAsync();
        Task<OrderDetailViewModel> GetOrderDetailsAsync(int orderId);

    }
}
