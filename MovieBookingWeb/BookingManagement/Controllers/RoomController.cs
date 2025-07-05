using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly BookingDbContext _db;

        public RoomController(BookingDbContext db)
        {
            _db = db;
        }

        // 1. Tạo phòng + sơ đồ ghế
        [HttpPost]
        public async Task<IActionResult> CreateRoom(CreateRoomDto dto)
        {
            var room = new Room
            {
                RoomName = dto.RoomName,
                RoomQuantity = dto.Rows * dto.Columns,
                Status = dto.Status
            };

            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();

            // Tạo ghế: rows = A, B, C... / columns = 1,2,3...
            for (int i = 0; i < dto.Rows; i++)
            {
                char rowChar = (char)('A' + i);
                for (int j = 1; j <= dto.Columns; j++)
                {
                    _db.Seats.Add(new Seat
                    {
                        RoomId = room.Id,
                        SeatRow = rowChar,
                        SeatColumn = (char)(j.ToString()[0]),
                        SeatStatus = true
                    });
                }
            }

            await _db.SaveChangesAsync();
            return Ok(new
            {
                Message = "Tạo phòng thành công.",
                RoomId = room.Id,
                room.RoomName,
                TotalSeats = room.RoomQuantity,
                Status = room.Status
            });
        }

        // 2. Cập nhật thông tin phòng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, UpdateRoomDto dto)
        {
            var room = await _db.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            room.RoomName = dto.RoomName ?? room.RoomName;
            room.Status = dto.Status ?? room.Status;
            await _db.SaveChangesAsync();
            return Ok(room);
        }

        // 3. Thống kê sử dụng phòng
        [HttpGet("statistics/{id}")]
        public async Task<IActionResult> GetRoomUtilization(int id)
        {
            var showtimes = await _db.Showtimes.Where(s => s.RoomId == id).ToListAsync();
            var stats = new List<object>();

            foreach (var show in showtimes)
            {
                var totalSeats = await _db.SeatShowtimes.CountAsync(s => s.ShowtimeId == show.Id);
                var booked = await _db.SeatShowtimes.CountAsync(s => s.ShowtimeId == show.Id && s.Status == SeatStatus.Booked);
                stats.Add(new
                {
                    show.ShowDate,
                    show.FromTime,
                    OccupancyRate = totalSeats == 0 ? 0 : booked * 100.0 / totalSeats
                });
            }

            return Ok(stats);
        }

        // 4. Lấy danh sách phòng
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _db.Rooms.ToListAsync();
            return Ok(rooms);
        }

        // 5. Lấy phòng theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await _db.Rooms.FindAsync(id);
            if (room == null)
                return NotFound("Không tìm thấy phòng.");

            return Ok(room);
        }
    }
}