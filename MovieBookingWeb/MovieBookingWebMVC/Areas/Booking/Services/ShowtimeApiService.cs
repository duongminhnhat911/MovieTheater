using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;
using Newtonsoft.Json;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public class ShowtimeApiService : IShowtimeApiService
    {
        private readonly HttpClient _httpClient;

        public ShowtimeApiService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ApiClient_Booking");
        }

        public async Task<List<ShowtimeDto>> GetAllShowtimesAsync()
        {
            var response = await _httpClient.GetAsync("api/showtime");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<ShowtimeDto>>(content) ?? new List<ShowtimeDto>();
        }

        public async Task<bool> CreateShowtimeAsync(CreateShowtimeDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/showtime", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
