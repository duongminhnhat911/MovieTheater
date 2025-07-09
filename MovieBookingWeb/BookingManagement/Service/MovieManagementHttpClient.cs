using BookingManagement.Models.DTOs;

namespace BookingManagement.Service
{
    public class MovieManagementHttpClient : IMovieServiceClient
    {
        private readonly HttpClient _httpClient;

        public MovieManagementHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MovieDto?> GetMovieById(int movieId)
        {
            var response = await _httpClient.GetAsync($"api/Movies/{movieId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<MovieDto>();
        }
    }
}