using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public interface IShowtimeApiService
    {
        Task<List<ShowtimeDto>> GetAllShowtimesAsync();
        Task<bool> CreateShowtimeAsync(CreateShowtimeDto dto);
    }
}
