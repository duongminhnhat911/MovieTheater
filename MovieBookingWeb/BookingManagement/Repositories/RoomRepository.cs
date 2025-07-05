using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly BookingDbContext _db;

        public RoomRepository(BookingDbContext db)
        {
            _db = db;
        }

        public async Task AddRoomAsync(Room room) =>
            await _db.Rooms.AddAsync(room);

        public async Task<Room?> GetRoomByIdAsync(int id) =>
            await _db.Rooms.FindAsync(id);

        public async Task<List<Room>> GetAllRoomsAsync() =>
            await _db.Rooms.ToListAsync();

        public async Task SaveChangesAsync() =>
            await _db.SaveChangesAsync();

        public async Task AddSeatAsync(Seat seat) =>
            await _db.Seats.AddAsync(seat);

        public async Task<List<Showtime>> GetShowtimesByRoomIdAsync(int roomId) =>
            await _db.Showtimes.Where(s => s.RoomId == roomId).ToListAsync();

        public async Task<int> CountSeatShowtimesAsync(int showtimeId) =>
            await _db.SeatShowtimes.CountAsync(s => s.ShowtimeId == showtimeId);

        public async Task<int> CountBookedSeatShowtimesAsync(int showtimeId) =>
            await _db.SeatShowtimes.CountAsync(s => s.ShowtimeId == showtimeId && s.Status == SeatStatus.Booked);
    }
}