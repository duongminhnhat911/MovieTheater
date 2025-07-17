using MovieBookingWebMVC.Areas.Booking.Models.DTOs;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public interface IShowtimeWebService
    {
        Task<List<ShowtimeDTOUser>> GetShowtimesByMovieIdAsync(int movieId);

        Task<List<ShowtimeDTOUser>> GetAllShowtimesAsync();
    }
}
