using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatShowtimeController : ControllerBase
    {
        private readonly BookingDbContext _db;

        public SeatShowtimeController(BookingDbContext db)
        {
            _db = db;
        }

        // 1. GET: Lấy tất cả SeatShowtime
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.SeatShowtimes
                .Include(s => s.Seat)
                .Include(s => s.Showtime)
                .ToListAsync();
            return Ok(list);
        }

        // 2. GET: Lấy chi tiết một SeatShowtime
        [HttpGet("{showtimeId}/{seatId}")]
        public async Task<IActionResult> Get(int showtimeId, int seatId)
        {
            var entity = await _db.SeatShowtimes
                .Include(s => s.Seat)
                .Include(s => s.Showtime)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId && s.SeatId == seatId);

            if (entity == null) return NotFound();
            return Ok(entity);
        }

        // 3. POST: Tạo mới SeatShowtime
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SeatShowtime seatShowtime)
        {
            _db.SeatShowtimes.Add(seatShowtime);
            await _db.SaveChangesAsync();
            return Ok(seatShowtime);
        }

        // 4. PUT: Cập nhật trạng thái ghế
        [HttpPut("{showtimeId}/{seatId}")]
        public async Task<IActionResult> Update(int showtimeId, int seatId, [FromBody] SeatStatus status)
        {
            var entity = await _db.SeatShowtimes
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId && s.SeatId == seatId);

            if (entity == null) return NotFound();

            entity.Status = status;
            await _db.SaveChangesAsync();

            return Ok(entity);
        }

        // 5. DELETE: Xoá SeatShowtime
        [HttpDelete("{showtimeId}/{seatId}")]
        public async Task<IActionResult> Delete(int showtimeId, int seatId)
        {
            var entity = await _db.SeatShowtimes
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId && s.SeatId == seatId);

            if (entity == null) return NotFound();

            _db.SeatShowtimes.Remove(entity);
            await _db.SaveChangesAsync();

            return Ok("Đã xoá thành công.");
        }
    }
}