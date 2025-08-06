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
                for (int j = 1; j < dto.Columns; j++) // j chạy từ 0 đến 9
                {
                    char columnChar = (char)('0' + j); // '0' → '9'
                    await _repo.AddSeatAsync(new Seat
                    {
                        RoomId = room.Id,
                        SeatRow = rowChar,
                        SeatColumn = columnChar,
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

            // Cập nhật thông tin cơ bản
            room.RoomName = dto.RoomName;
            room.Status = dto.Status;

            // Lấy ghế hiện tại để kiểm tra sơ đồ
            var currentSeats = await _repo.GetSeatsByRoomIdAsync(room.Id);
            var currentRows = currentSeats.Select(s => s.SeatRow).Distinct().Count();
            var currentColumns = currentSeats.Select(s => s.SeatColumn).Distinct().Count();

            // Nếu có thay đổi sơ đồ thì cập nhật lại ghế
            if (currentRows != dto.Rows || currentColumns != dto.Columns)
            {
                room.RoomQuantity = dto.Rows * dto.Columns;

                await _repo.RemoveSeatsByRoomIdAsync(room.Id);

                for (int i = 0; i < dto.Rows; i++)
                {
                    char rowChar = (char)('A' + i);
                    for (int j = 0; j < dto.Columns; j++) // bắt đầu từ 0 như CreateRoomAsync
                    {
                        char columnChar = (char)('0' + j);
                        await _repo.AddSeatAsync(new Seat
                        {
                            RoomId = room.Id,
                            SeatRow = rowChar,
                            SeatColumn = columnChar,
                            SeatStatus = true
                        });
                    }
                }
            }

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
        public async Task<List<Seat>> GetSeatsByRoomIdAsync(int roomId)
        {
            return await _repo.GetSeatsByRoomIdAsync(roomId);
        }

        public async Task<List<Room>> GetRoomsAsync() =>
            await _repo.GetAllRoomsAsync();

        public async Task<Room?> GetRoomByIdAsync(int id) =>
            await _repo.GetRoomByIdAsync(id);
        public async Task<RoomDetailsDto?> GetRoomDetailsAsync(int roomId)
        {
            return await _repo.GetRoomDetailsByIdAsync(roomId);
        }
    }
}
