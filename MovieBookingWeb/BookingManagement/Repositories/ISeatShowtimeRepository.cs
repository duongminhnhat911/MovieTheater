using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;

namespace BookingManagement.Repositories
{
    public interface ISeatShowtimeRepository
    {
        Task<List<SeatShowtime>> GetAllAsync();
        Task<SeatShowtime?> GetAsync(int showtimeId, int seatId);
        Task AddAsync(SeatShowtime seatShowtime);
        Task SaveChangesAsync();
        void Remove(SeatShowtime seatShowtime);
        Task HoldSeatAsync(int seatId, int showtimeId);
        Task<Dictionary<int, SeatStatus>> GetSeatStatusesAsync(int showtimeId);
    }
}
