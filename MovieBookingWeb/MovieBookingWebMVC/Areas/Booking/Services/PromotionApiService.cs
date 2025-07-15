using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
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
        // PromotionApiService.cs
        public async Task<bool> CreatePromotionAsync(CreatePromotionViewModel model)
        {
            var dto = new CreatePromotionDto
            {
                PromotionCode = model.PromotionCode,
                Description = model.Description,
                DiscountAmount = model.DiscountAmount,
                DiscountPercent = model.DiscountPercent,
                Quantity = model.Quantity,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            var response = await _httpClient.PostAsJsonAsync("api/promotion", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePromotionAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Promotion/{id}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var error = await response.Content.ReadAsStringAsync();        
            return false;
        }

    }
}
