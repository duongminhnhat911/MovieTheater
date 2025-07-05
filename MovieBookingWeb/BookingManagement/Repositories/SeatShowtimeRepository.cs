using BookingManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Repositories
{
    // Repositories/SeatShowtimeRepository.cs
    public class SeatShowtimeRepository : ISeatShowtimeRepository
    {
        private readonly BookingDbContext _db;

        public SeatShowtimeRepository(BookingDbContext db)
        {
            _db = db;
        }

        public async Task<List<SeatShowtime>> GetAllAsync()
        {
            return await _db.SeatShowtimes
                .Include(s => s.Seat)
                .Include(s => s.Showtime)
                .ToListAsync();
        }

        public async Task<SeatShowtime?> GetAsync(int showtimeId, int seatId)
        {
            return await _db.SeatShowtimes
                .Include(s => s.Seat)
                .Include(s => s.Showtime)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId && s.SeatId == seatId);
        }

        public async Task AddAsync(SeatShowtime seatShowtime)
        {
            await _db.SeatShowtimes.AddAsync(seatShowtime);
        }

        public void Remove(SeatShowtime seatShowtime)
        {
            _db.SeatShowtimes.Remove(seatShowtime);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
