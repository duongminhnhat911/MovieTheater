using BookingManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly BookingDbContext _db;

        public SeatRepository(BookingDbContext db)
        {
            _db = db;
        }

        public async Task<List<Seat>> GetSeatsByRoomAsync(int roomId) =>
            await _db.Seats
                    .Where(s => s.RoomId == roomId)
                    .Include(s => s.Room)
                    .ToListAsync();

        public async Task<Seat?> GetSeatByIdAsync(int id) =>
            await _db.Seats.FindAsync(id);

        public async Task<Seat?> GetSeatWithRoomAsync(int id) =>
            await _db.Seats
                    .Include(s => s.Room)
                    .FirstOrDefaultAsync(s => s.Id == id);

        public async Task SaveChangesAsync() =>
            await _db.SaveChangesAsync();
    }
}