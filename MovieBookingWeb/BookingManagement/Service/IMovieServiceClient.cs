using BookingManagement.Models.DTOs;

namespace BookingManagement.Service
{
    public interface IMovieServiceClient
    {
        Task<MovieDto?> GetMovieById(int movieId);
    }
}
