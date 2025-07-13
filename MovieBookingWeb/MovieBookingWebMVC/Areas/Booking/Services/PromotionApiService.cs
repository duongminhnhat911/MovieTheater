using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public class PromotionApiService : IPromotionApiService
    {
        private readonly HttpClient _httpClient;

        public PromotionApiService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ApiClient_Booking");
        }
        public async Task<List<PromotionListViewModel>> GetAllPromotionsAsync()
        {
            var response = await _httpClient.GetAsync("/api/Promotion");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<PromotionListViewModel>>() ?? new List<PromotionListViewModel>();
        }
        public async Task<PromotionListViewModel?> GetPromotionByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Promotion/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PromotionListViewModel>();
            }
            return null;
        }

        public async Task<bool> UpdatePromotionAsync(int id, PromotionListViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Promotion/{id}", model);
            return response.IsSuccessStatusCode;
        }
    }
}
