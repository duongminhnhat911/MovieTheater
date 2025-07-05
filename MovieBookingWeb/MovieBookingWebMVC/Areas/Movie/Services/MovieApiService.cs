using MovieBookingWebMVC.Areas.Movie.Models.DTOs;
using Newtonsoft.Json;
using MovieBookingWebMVC.Areas.Movie.Models.ViewModel;

namespace MovieBookingWebMVC.Areas.Movie.Services
{
    public class MovieApiService
    {
        private readonly HttpClient _httpClient;

        public MovieApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient_Movie");
        }

        public async Task<List<FilmViewModel>> GetMovies()
        {
            var response = await _httpClient.GetAsync("api/movies");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FilmViewModel>>(content) ?? new List<FilmViewModel>();
        }

        public async Task<FilmViewModel?> GetMovie(int id)
        {
            var response = await _httpClient.GetAsync($"api/movies/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FilmViewModel>(content);
        }

        public async Task AddMovie(MovieCreateDto movieDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(movieDto), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/movies", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateMovie(MovieCreateDto movieDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(movieDto), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/movies/{movieDto.Id}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteMovie(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/movies/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}