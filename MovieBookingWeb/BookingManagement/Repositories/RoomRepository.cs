using BookingManagement.Models.DTOs;
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

        public async Task<List<Seat>> GetSeatsByRoomIdAsync(int roomId)
        {
            return await _db.Seats.Where(s => s.RoomId == roomId).ToListAsync();
        }

        public async Task RemoveSeatsByRoomIdAsync(int roomId)
        {
            var seats = _db.Seats.Where(s => s.RoomId == roomId);
            _db.Seats.RemoveRange(seats);
            await _db.SaveChangesAsync();
        }
        public async Task<RoomDetailsDto?> GetRoomDetailsByIdAsync(int roomId)
        {
            var room = await _db.Rooms.FindAsync(roomId);
            if (room == null) return null;

            var seats = await _db.Seats
            .Where(s => s.RoomId == roomId)
            .OrderBy(s => s.SeatRow)   // Ưu tiên sắp xếp theo Row trước
            .ThenBy(s => s.SeatColumn) // Sau đó là Column
            .Select(s => $"{s.SeatRow}{s.SeatColumn}")
            .ToListAsync();

            return new RoomDetailsDto
            {
                Id = room.Id,
                RoomName = room.RoomName,
                RoomQuantity = room.RoomQuantity,
                Status = room.Status,
                Seats = seats
            };
        }
    }
}