using MovieBookingWebMVC.Areas.Booking.Models.DTOs;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public interface IBookingApiService
    {
        Task<ShowtimeDTO?> GetScheduleDetailAsync(int scheduleId);
        Task<SeatShowtimeDTO> GetSeatSchedulesAsync(int showtimeId);

    }
}