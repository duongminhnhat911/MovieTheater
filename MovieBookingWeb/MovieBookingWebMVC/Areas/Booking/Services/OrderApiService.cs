using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;
using MovieBookingWebMVC.Areas.Movie.Services;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public class OrderApiService : IOrderApiService
    {
        private readonly HttpClient _httpClient;
        private readonly MovieApiService _movieApiService;

        public OrderApiService(IHttpClientFactory factory, MovieApiService movieApiService)
        {
            _httpClient = factory.CreateClient("ApiClient_Booking");
            _movieApiService = movieApiService;
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

            var detail = await response.Content.ReadFromJsonAsync<OrderDetailViewModel>()
                         ?? new OrderDetailViewModel();


            if (detail.MovieId > 0)
            {
                var movie = await _movieApiService.GetMovie(detail.MovieId);
                if (movie != null)
                    detail.MovieTitle = movie.Title;
            }

            return detail;
        }
    }
}
