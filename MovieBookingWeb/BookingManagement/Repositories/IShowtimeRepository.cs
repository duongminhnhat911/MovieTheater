using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;

namespace BookingManagement.Repositories
{
    public interface IShowtimeRepository
    {
        Task<Room?> GetActiveRoomAsync(int roomId);
        Task<List<Seat>> GetSeatsByRoomAsync(int roomId);
        Task<MovieDto?> GetMovieByIdAsync(int movieId);
        Task<List<Showtime>> GetOverlappingShowtimesAsync(int roomId, DateOnly date);
        Task<List<Showtime>> GetAllShowtimesAsync();
        Task<Showtime?> GetShowtimeByIdAsync(int id);
        Task<int> CountTotalSeatsAsync(int showtimeId);
        Task<int> CountOccupiedSeatsAsync(int showtimeId);
        Task<List<OrderDetail>> GetOrderDetailsByShowtimeIdAsync(int showtimeId);
        Task<List<int>> GetOrderIdsByShowtimeIdAsync(int showtimeId);
        void AddShowtime(Showtime showtime);
        void AddSeatShowtime(SeatShowtime seatShowtime);
        void RemoveSeatShowtimes(int showtimeId);
        void RemoveShowtime(Showtime showtime);
        Task SaveChangesAsync();
    }
}
