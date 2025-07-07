using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using MovieBookingWebMVC.Areas.Booking.Models;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public interface IBookingApiService
    {
        Task<ShowTime?> GetScheduleDetailAsync(int scheduleId);
        Task<SeatShowtimeDTO> GetSeatSchedulesAsync(int showtimeId);

    }
}