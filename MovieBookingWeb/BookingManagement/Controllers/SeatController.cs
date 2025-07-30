using BookingManagement.Models.DTOs;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.Controllers
{
    // Controllers/SeatController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _service;

        public SeatController(ISeatService service)
        {
            _service = service;
        }

        // 1. Xem danh sách ghế trong phòng
        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetSeatsByRoom(int roomId)
        {
            var result = await _service.GetSeatsByRoomAsync(roomId);
            return Ok(result);
        }

        // 2. Cập nhật cấu hình 1 ghế
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeat(int id, UpdateSeatDto dto)
        {
            var seat = await _service.UpdateSeatAsync(id, dto);
            if (seat == null) return NotFound();
            return Ok(seat);
        }

        // 3. Xóa mềm ghế
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteSeat(int id)
        {
            var result = await _service.SoftDeleteSeatAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // 4. Khôi phục ghế
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> RestoreSeat(int id)
        {
            var success = await _service.RestoreSeatAsync(id);
            if (!success) return NotFound();
            return Ok("Đã khôi phục ghế.");
        }
    }

}