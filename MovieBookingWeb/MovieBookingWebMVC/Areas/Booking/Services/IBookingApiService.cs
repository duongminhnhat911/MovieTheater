using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using MovieBookingWebMVC.Areas.Booking.Models.ViewModels;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public interface IBookingApiService
    {
        Task<ShowtimeDTOUser?> GetScheduleDetailAsync(int scheduleId);
        Task<SeatShowtimeDTO> GetSeatSchedulesAsync(int showtimeId);
        Task<ConfirmBookingViewModel?> MarkOrderAsBookedAsync(int orderId);
    }
}