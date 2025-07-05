using BookingManagement.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly BookingDbContext _db;

        public SeatController(BookingDbContext db)
        {
            _db = db;
        }

        // 1. Xem danh sách ghế trong phòng
        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetSeatsByRoom(int roomId)
        {
            var seats = await _db.Seats
                .Where(s => s.RoomId == roomId)
                .Include(s => s.Room) // 👈 Join với bảng Room để lấy tên phòng
                .ToListAsync();

            var result = seats.Select(s => new
            {
                s.Id,
                s.SeatRow,
                s.SeatColumn,
                s.SeatStatus,
                RoomName = s.Room?.RoomName
            });

            return Ok(result);
        }

        // 2. Cập nhật cấu hình 1 ghế
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeat(int id, UpdateSeatDto dto)
        {
            var seat = await _db.Seats.FindAsync(id);
            if (seat == null) return NotFound();

            seat.SeatStatus = dto.SeatStatus ?? seat.SeatStatus;
            await _db.SaveChangesAsync();
            return Ok(seat);
        }

        // 3. Xóa mềm ghế (tức là khóa ghế)
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteSeat(int id)
        {
            var seat = await _db.Seats
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seat == null) return NotFound();

            seat.SeatStatus = false;
            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "Đã xóa mềm ghế (đã khóa).",
                SeatId = seat.Id,
                Room = seat.Room?.RoomName
            });

        }
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> RestoreSeat(int id)
        {
            var seat = await _db.Seats.FindAsync(id);
            if (seat == null) return NotFound();

            seat.SeatStatus = true;
            await _db.SaveChangesAsync();

            return Ok("Đã khôi phục ghế.");
        }
    }
}