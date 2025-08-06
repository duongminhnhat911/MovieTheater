using MovieBookingWebMVC.Areas.Booking.Models.DTOs; 
using System.Text.Json;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public class ShowtimeWebService : IShowtimeWebService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ShowtimeWebService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ShowtimeDTOUser>> GetShowtimesByMovieIdAsync(int movieId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient_Booking");
            var movieClient = _httpClientFactory.CreateClient("ApiClient_Movie");

            // Gọi tất cả lịch chiếu
            var response = await client.GetAsync($"/api/showtime");
            if (!response.IsSuccessStatusCode)
                return new List<ShowtimeDTOUser>();

            var rawList = await response.Content.ReadFromJsonAsync<List<ShowtimeRawDTO>>();
            if (rawList == null) return new List<ShowtimeDTOUser>();

            // Lọc theo movieId
            var filteredRawList = rawList.Where(s => s.MovieId == movieId).ToList();

            // Gọi API lấy thông tin phim
            MovieDTO? movie = null;
            var movieRes = await movieClient.GetAsync($"/api/Movies/{movieId}");
            if (movieRes.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                movie = await movieRes.Content.ReadFromJsonAsync<MovieDTO>(options);
            }

            return await MapShowtimesWithAvailableSeats(client, filteredRawList, movie);
        }

        private async Task<List<ShowtimeDTOUser>> MapShowtimesWithAvailableSeats(HttpClient client, List<ShowtimeRawDTO> rawList, MovieDTO? movie)
        {
            var result = new List<ShowtimeDTOUser>();

            foreach (var raw in rawList)
            {
                var showtime = new ShowtimeDTOUser
                {
                    Id = raw.Id,
                    MovieId = raw.MovieId,
                    ShowDate = DateOnly.Parse(raw.ShowDate),
                    FromTime = TimeSpan.Parse(raw.FromTime),
                    ToTime = TimeSpan.Parse(raw.ToTime),
                    MovieTitle = movie?.Title ?? string.Empty,
                    MoviePoster = movie?.Image ?? string.Empty,
                    Subtitle = movie?.Subtitle ?? string.Empty,
                    Director = movie?.Director ?? string.Empty,
                    Duration = movie?.Duration,
                    Genres = movie?.Genres ?? new List<string>(),
                    Format = movie?.Format ?? new List<string>()
                };

                // Lấy thông tin ghế
                var seatRes = await client.GetAsync($"/api/SeatShowtime/seats/showtime?showtimeId={raw.Id}");
                if (seatRes.IsSuccessStatusCode)
                {
                    var seatData = await seatRes.Content.ReadFromJsonAsync<SeatShowtimeResult>();
                    if (seatData?.Seats != null)
                    {
                        showtime.AvailableSeats = seatData.Seats.Count(s => s.Status == "Available");
                    }
                }

                result.Add(showtime);
            }

            return result;
        }

        public async Task<List<ShowtimeDTOUser>> GetAllShowtimesAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient_Booking");
            var response = await client.GetAsync("/api/Showtime");

            if (!response.IsSuccessStatusCode)
                return new List<ShowtimeDTOUser>();

            var rawList = await response.Content.ReadFromJsonAsync<List<ShowtimeRawDTO>>();
            if (rawList == null) return new List<ShowtimeDTOUser>();

            return await MapShowtimesWithAvailableSeats(client, rawList, null);
        }

        // Raw DTO từ API /api/showtime
        private class ShowtimeRawDTO
        {
            public int Id { get; set; }
            public int MovieId { get; set; }
            public string ShowDate { get; set; } = string.Empty;
            public string FromTime { get; set; } = string.Empty;
            public string ToTime { get; set; } = string.Empty;
            public string? Room { get; set; }
        }

        private class SeatShowtimeResult
        {
            public int ShowtimeId { get; set; }
            public int RoomId { get; set; }
            public List<SeatDTOUser> Seats { get; set; } = new();
        }
    }
}
