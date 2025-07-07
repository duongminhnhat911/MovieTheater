using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using System.Net.Http;
using System.Net.Http.Json;

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

            // ❌ Không truyền movieId
            var response = await client.GetAsync($"/api/showtime");
            if (!response.IsSuccessStatusCode)
                return new List<ShowtimeDTOUser>();

            var rawList = await response.Content.ReadFromJsonAsync<List<ShowtimeRawDTO>>();
            if (rawList == null) return new List<ShowtimeDTOUser>();

            // ✅ Lọc theo movieId sau khi đã lấy toàn bộ
            var filteredRawList = rawList.Where(s => s.MovieId == movieId).ToList();

            // Lấy thông tin tiêu đề phim
            string? movieTitle = null;
            var movieRes = await movieClient.GetAsync($"/api/movie/{movieId}");
            if (movieRes.IsSuccessStatusCode)
            {
                var movie = await movieRes.Content.ReadFromJsonAsync<MovieDTO>();
                movieTitle = movie?.Title;
            }

            return await MapShowtimesWithAvailableSeats(client, filteredRawList, movieTitle);
        }
        private async Task<List<ShowtimeDTOUser>> MapShowtimesWithAvailableSeats(HttpClient client, List<ShowtimeRawDTO> rawList, string movieTitle = "")
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
                    MovieTitle = movieTitle
                };

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

            // Gán tên phim trống vì gọi tất cả lịch chiếu nhiều phim
            return await MapShowtimesWithAvailableSeats(client, rawList, "");
        }
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