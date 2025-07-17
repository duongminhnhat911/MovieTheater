using MovieBookingWebMVC.Areas.Booking.Models.DTOs;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public interface ISeatWebService
    {
        Task<SeatShowtimeDTO?> GetSeatsByShowtimeAsync(int showtimeId);
    }
}
