using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
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
                 .ThenInclude(st => st.Room)
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
        public async Task HoldSeatAsync(int seatId, int showtimeId)
        {
            var seatShowtime = await _db.SeatShowtimes
                .FirstOrDefaultAsync(ss => ss.SeatId == seatId && ss.ShowtimeId == showtimeId);

            if (seatShowtime != null)
            {
                seatShowtime.Status = SeatStatus.Held;
                await _db.SaveChangesAsync();
            }
        }
        public async Task<Dictionary<int, SeatStatus>> GetSeatStatusesAsync(int showtimeId)
        {
            return await _db.SeatShowtimes
                .Where(ss => ss.ShowtimeId == showtimeId)
                .ToDictionaryAsync(ss => ss.SeatId, ss => ss.Status);
        }
    }
}
