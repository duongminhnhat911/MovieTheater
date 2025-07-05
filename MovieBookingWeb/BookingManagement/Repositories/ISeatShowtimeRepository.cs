using BookingManagement.Models.Entities;

namespace BookingManagement.Repositories
{
    public interface ISeatShowtimeRepository
    {
        Task<List<SeatShowtime>> GetAllAsync();
        Task<SeatShowtime?> GetAsync(int showtimeId, int seatId);
        Task AddAsync(SeatShowtime seatShowtime);
        Task SaveChangesAsync();
        void Remove(SeatShowtime seatShowtime);
    }
}
