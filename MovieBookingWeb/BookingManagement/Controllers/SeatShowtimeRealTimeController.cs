using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BookingManagement.Controllers
{
    [ApiController]
    [Route("api/realtime/seatshowtime")]
    public class SeatShowtimeRealTimeController : ControllerBase
    {
        private readonly ISeatShowtimeService _service;
        private readonly IHubContext<SeatHub> _hub;

        public SeatShowtimeRealTimeController(ISeatShowtimeService service, IHubContext<SeatHub> hub)
        {
            _service = service;
            _hub = hub;
        }

        [HttpPost("lock")]
        public async Task<IActionResult> LockSeat([FromBody] LockSeatShowtimeRequest dto)
        {
            var seat = await _service.GetAsync(dto.ShowtimeId, dto.SeatId);
            if (seat == null)
                return NotFound("Ghế không tồn tại.");

            if (seat.Status != SeatStatus.Available)
                return Conflict("Ghế đã bị giữ hoặc đặt.");

            seat.Status = SeatStatus.Held;
            seat.HeldByUserId = dto.UserId;
            seat.HeldUntil = DateTime.UtcNow.AddMinutes(3);

            await _service.UpdateEntityAsync(seat);

            await _hub.Clients.Group($"showtime-{dto.ShowtimeId}")
                .SendAsync("SeatLocked", dto.SeatId, dto.UserId);

            return Ok("Đã giữ ghế trong 3 phút.");
        }

        [HttpPost("unlock")]
        public async Task<IActionResult> UnlockSeat([FromBody] LockSeatShowtimeRequest dto)
        {
            var seat = await _service.GetAsync(dto.ShowtimeId, dto.SeatId);
            if (seat == null)
                return NotFound();

            if (seat.Status == SeatStatus.Held && seat.HeldByUserId == dto.UserId)
            {
                seat.Status = SeatStatus.Available;
                seat.HeldByUserId = null;
                seat.HeldUntil = null;

                await _service.UpdateEntityAsync(seat);

                await _hub.Clients.Group($"showtime-{dto.ShowtimeId}")
                    .SendAsync("SeatUnlocked", dto.SeatId);

                return Ok("Đã mở khóa ghế.");
            }

            return BadRequest("Không có quyền mở khóa hoặc trạng thái không hợp lệ.");
        }

        [HttpGet("held-until")]
        public async Task<IActionResult> GetHeldUntil([FromQuery] int showtimeId, [FromQuery] int seatId)
        {
            var seat = await _service.GetAsync(showtimeId, seatId);
            if (seat == null || seat.Status != SeatStatus.Held)
                return NotFound("Ghế không được giữ.");

            return Ok(new
            {
                HeldUntil = seat.HeldUntil,
                HeldByUserId = seat.HeldByUserId
            });
        }
    }


    public class LockSeatShowtimeRequest
    {
        public int ShowtimeId { get; set; }
        public int SeatId { get; set; }
        public int UserId { get; set; }
    }
}
