using MovieBookingWebMVC.Areas.Booking.Models.DTOs;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public interface IShowtimeWebService
    {
        Task<List<ShowtimeDTO>> GetShowtimesByMovieIdAsync(int movieId);

        Task<List<ShowtimeDTO>> GetAllShowtimesAsync();
    }
}
