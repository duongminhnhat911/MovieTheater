using MovieBookingWeb.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieBookingWeb.Services
{
    public class MovieApiService
    {
        private readonly HttpClient _httpClient;

        public MovieApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MovieApiClient");
        }

        public async Task<List<Film>> GetMovies()
        {
            var response = await _httpClient.GetAsync("api/movies");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Film>>(content) ?? new List<Film>();
        }

        public async Task<Film?> GetMovie(int id)
        {
            var response = await _httpClient.GetAsync($"api/movies/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Film>(content);
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