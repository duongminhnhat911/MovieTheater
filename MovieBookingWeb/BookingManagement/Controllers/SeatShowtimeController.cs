using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.Controllers
{
    // Controllers/SeatShowtimeController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class SeatShowtimeController : ControllerBase
    {
        private readonly ISeatShowtimeService _service;

        public SeatShowtimeController(ISeatShowtimeService service)
        {
            _service = service;
        }

        // 1. GET: Lấy tất cả SeatShowtime
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // 2. GET: Lấy chi tiết một SeatShowtime
        [HttpGet("{showtimeId}/{seatId}")]
        public async Task<IActionResult> Get(int showtimeId, int seatId)
        {
            var entity = await _service.GetAsync(showtimeId, seatId);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        // 3. POST: Tạo mới SeatShowtime
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SeatShowtime seatShowtime)
        {
            var result = await _service.CreateAsync(seatShowtime);
            return Ok(result);
        }

        // 4. PUT: Cập nhật trạng thái ghế
        [HttpPut("{showtimeId}/{seatId}")]
        public async Task<IActionResult> Update(int showtimeId, int seatId, [FromBody] SeatStatus status)
        {
            var result = await _service.UpdateStatusAsync(showtimeId, seatId, status);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // 5. DELETE: Xoá SeatShowtime
        [HttpDelete("{showtimeId}/{seatId}")]
        public async Task<IActionResult> Delete(int showtimeId, int seatId)
        {
            var success = await _service.DeleteAsync(showtimeId, seatId);
            if (!success) return NotFound();
            return Ok("Đã xoá thành công.");
        }
        [HttpGet("seats/showtime")]
        public async Task<IActionResult> GetSeatsByShowtime(int showtimeId)
        {
            var result = await _service.GetSeatsByShowtimeAsync(showtimeId);
            if (result == null)
                return NotFound("Showtime không tồn tại.");

            return Ok(result);
        }

    }
}
    