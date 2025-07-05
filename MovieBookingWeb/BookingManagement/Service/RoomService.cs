using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Repositories;

namespace BookingManagement.Service
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _repo;

        public RoomService(IRoomRepository repo)
        {
            _repo = repo;
        }

        public async Task<object> CreateRoomAsync(CreateRoomDto dto)
        {
            var room = new Room
            {
                RoomName = dto.RoomName,
                RoomQuantity = dto.Rows * dto.Columns,
                Status = dto.Status
            };

            await _repo.AddRoomAsync(room);
            await _repo.SaveChangesAsync();

            for (int i = 0; i < dto.Rows; i++)
            {
                char rowChar = (char)('A' + i);
                for (int j = 1; j <= dto.Columns; j++)
                {
                    await _repo.AddSeatAsync(new Seat
                    {
                        RoomId = room.Id,
                        SeatRow = rowChar,
                        SeatColumn = (char)(j.ToString()[0]),
                        SeatStatus = true
                    });
                }
            }

            await _repo.SaveChangesAsync();

            return new
            {
                Message = "Tạo phòng thành công.",
            };
        }

        public async Task<Room?> UpdateRoomAsync(int id, UpdateRoomDto dto)
        {
            var room = await _repo.GetRoomByIdAsync(id);
            if (room == null) return null;

            room.RoomName = dto.RoomName ?? room.RoomName;
            room.Status = dto.Status ?? room.Status;

            await _repo.SaveChangesAsync();
            return room;
        }

        public async Task<List<object>> GetRoomUtilizationAsync(int id)
        {
            var showtimes = await _repo.GetShowtimesByRoomIdAsync(id);
            var stats = new List<object>();

            foreach (var show in showtimes)
            {
                var totalSeats = await _repo.CountSeatShowtimesAsync(show.Id);
                var booked = await _repo.CountBookedSeatShowtimesAsync(show.Id);
                stats.Add(new
                {
                    show.ShowDate,
                    show.FromTime,
                    OccupancyRate = totalSeats == 0 ? 0 : booked * 100.0 / totalSeats
                });
            }

            return stats;
        }

        public async Task<List<Room>> GetRoomsAsync() =>
            await _repo.GetAllRoomsAsync();

        public async Task<Room?> GetRoomByIdAsync(int id) =>
            await _repo.GetRoomByIdAsync(id);
    }
}
