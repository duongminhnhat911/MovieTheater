using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using System.Net.Http.Json;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public class SeatWebService : ISeatWebService
    {
        private readonly IHttpClientFactory _factory;

        public SeatWebService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<SeatShowtimeDTO?> GetSeatsByShowtimeAsync(int showtimeId)
        {
            var client = _factory.CreateClient("ApiClient_Booking");
            var response = await client.GetAsync($"/api/SeatShowtime/seats/showtime?showtimeId={showtimeId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<SeatShowtimeDTO>();
        }
    }
}
