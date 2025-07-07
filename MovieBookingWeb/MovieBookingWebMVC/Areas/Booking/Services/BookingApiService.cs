using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using MovieBookingWebMVC.Areas.Booking.Models;
using MovieBookingWebMVC.Areas.Booking.Models.DTOs;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public class BookingApiService : IBookingApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BookingApiService> _logger;

        public BookingApiService(IHttpClientFactory httpClientFactory, ILogger<BookingApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ShowTime?> GetScheduleDetailAsync(int scheduleId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient_Booking");
                var response = await client.GetAsync($"/api/showtime/{scheduleId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Không thể lấy lịch chiếu ID {ScheduleId}", scheduleId);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<ShowTime>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi GetScheduleDetailAsync");
                return null;
            }
        }

        public async Task<SeatShowtimeDTO> GetSeatSchedulesAsync(int showtimeId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient_Booking");
                var response = await client.GetAsync($"/api/SeatShowtime/seats/showtime?showtimeId={showtimeId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Không thể lấy danh sách ghế cho suất chiếu {ShowtimeId}", showtimeId);
                    return new SeatShowtimeDTO();
                }

                return await response.Content.ReadFromJsonAsync<SeatShowtimeDTO>() ?? new SeatShowtimeDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi GetSeatSchedulesAsync");
                return new SeatShowtimeDTO();
            }
        }

       
    }
}