using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using MovieBookingWebMVC.Areas.Booking.Models.ViewModels;
using MovieBookingWebMVC.Areas.Movie.Services;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public class BookingApiService : IBookingApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BookingApiService> _logger;
        private readonly MovieApiService _movieApiService;

        public BookingApiService(
            IHttpClientFactory httpClientFactory,
            ILogger<BookingApiService> logger,
            MovieApiService movieApiService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _movieApiService = movieApiService;
        }

        public async Task<ShowtimeDTOUser?> GetScheduleDetailAsync(int scheduleId)
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

                return await response.Content.ReadFromJsonAsync<ShowtimeDTOUser>();
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
        // ✅ HÀM MỚI: Đánh dấu đơn hàng đã đặt (Booked)
        public async Task<ConfirmBookingViewModel?> MarkOrderAsBookedAsync(int orderId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient_Booking");

                // 1. Lấy đơn hàng
                var orderRes = await client.GetAsync($"api/Order/{orderId}");
                if (!orderRes.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Không lấy được đơn hàng {OrderId}", orderId);
                    return null;
                }

                var order = await orderRes.Content.ReadFromJsonAsync<OrderDTO>();
                if (order == null) return null;

                // 2. Cập nhật trạng thái
                order.Status = true;
                var updateRes = await client.PutAsJsonAsync($"api/Order/{orderId}", order);
                if (!updateRes.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Không thể cập nhật trạng thái đơn hàng {OrderId}", orderId);
                    return null;
                }

                // 3. Lấy OrderDetail
                var detailRes = await client.GetAsync("api/OrderDetail");
                if (!detailRes.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Không lấy được chi tiết đơn hàng.");
                    return null;
                }

                var allDetails = await detailRes.Content.ReadFromJsonAsync<List<OrderDetailDTO>>();
                var orderDetails = allDetails?.Where(d => d.Order.Id == orderId).ToList();
                if (orderDetails == null || !orderDetails.Any()) return null;

                var detail = orderDetails.First();
                var movie = await _movieApiService.GetMovie(detail.Showtime.MovieId);
                if (movie == null) return null;

                return new ConfirmBookingViewModel
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    BookingDate = order.BookingDate,
                    TotalPrice = order.TotalPrice,
                    Status = true,
                    Movie = new MovieDTO
                    {
                        Id = movie.Id,
                        Title = movie.Title,
                        Subtitle = movie.Subtitle,
                        Image = movie.Image
                    },
                    Showtime = new ShowtimeDTOUser
                    {
                        Id = detail.Showtime.Id,
                        RoomName = detail.Showtime.RoomName,
                        ShowDate = detail.Showtime.ShowDate,
                        FromTime = detail.Showtime.FromTime
                    },
                    SelectedSeats = orderDetails.Select(d => d.Seat).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý MarkOrderAsBookedAsync cho đơn hàng {OrderId}", orderId);
                return null;
            }

        }

    }
}