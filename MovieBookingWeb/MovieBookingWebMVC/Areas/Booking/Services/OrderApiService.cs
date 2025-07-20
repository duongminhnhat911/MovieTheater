using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public class OrderApiService : IOrderApiService
    {
        private readonly HttpClient _httpClient;

        public OrderApiService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ApiClient_Booking");
        }

        public async Task<List<OrderSummaryViewModel>> GetAllOrdersAsync()
        {
            var response = await _httpClient.GetAsync("/api/Order");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<OrderSummaryViewModel>>()
                ?? new List<OrderSummaryViewModel>();
        }

        public async Task<OrderDetailViewModel> GetOrderDetailsAsync(int orderId)
        {
            var response = await _httpClient.GetAsync($"/api/OrderDetail/full/{orderId}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<OrderDetailViewModel>()
                   ?? new OrderDetailViewModel();
        }
    }
}
