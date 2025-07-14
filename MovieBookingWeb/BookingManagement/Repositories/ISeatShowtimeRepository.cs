using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using Microsoft.EntityFrameworkCore;

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
        DbSet<SeatShowtime> SeatShowtimes { get; }  // 👈 THÊM DÒNG NÀY

    }
}
