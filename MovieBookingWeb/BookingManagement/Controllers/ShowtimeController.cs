using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimeController : ControllerBase
    {
        private readonly BookingDbContext _db;
        private readonly IMovieServiceClient _movieService;

        public ShowtimeController(BookingDbContext db, IMovieServiceClient movieService)
        {
            _db = db;
            _movieService = movieService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShowtime([FromBody] CreateShowtimeDto dto)
        {
            // 1. Kiểm tra phòng
            var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Id == dto.RoomId && r.Status);
            if (room == null) return BadRequest("Phòng không tồn tại hoặc đã bị khóa.");

            // 2. Lấy thông tin phim từ MovieManagement
            var movie = await _movieService.GetMovieById(dto.MovieId);
            if (movie == null || movie.Status != "Active")
                return BadRequest("Phim không tồn tại hoặc đã ngừng chiếu.");
            if (movie.Duration == null)
                return BadRequest("Phim chưa có thời lượng.");

            // 3. Tính thời gian
            const int buffer = 15;
            var startTime = dto.StartTime;
            var endTime = startTime.AddMinutes(movie.Duration.Value + buffer);

            // 4. Check vượt quá giờ cho phép
            TimeOnly closing = new(23, 59);
            if (endTime > closing)
                return BadRequest("Suất chiếu kết thúc sau giờ hoạt động của rạp.");

            // 5. Kiểm tra trùng suất chiếu
            var overlapping = await _db.Showtimes
                .Where(s => s.RoomId == dto.RoomId && s.ShowDate == dto.ShowDate)
                .ToListAsync();

            foreach (var s in overlapping)
            {
                if (!(startTime >= s.ToTime || endTime <= s.FromTime))
                    return Conflict("Suất chiếu bị trùng với lịch đã có.");
            }

            // 6. Tạo suất chiếu
            var showtime = new Showtime
            {
                MovieId = dto.MovieId,
                RoomId = dto.RoomId,
                ShowDate = dto.ShowDate,
                FromTime = startTime,
                ToTime = endTime
            };

            _db.Showtimes.Add(showtime);
            await _db.SaveChangesAsync();

            // 7. Gán ghế cho suất chiếu
            var seats = await _db.Seats.Where(s => s.RoomId == dto.RoomId).ToListAsync();
            foreach (var seat in seats)
            {
                _db.SeatShowtimes.Add(new SeatShowtime
                {
                    ShowtimeId = showtime.Id,
                    SeatId = seat.Id,
                    Status = SeatStatus.Available
                });
            }

            await _db.SaveChangesAsync();

            return Ok(new { showtime.Id, TotalSeats = seats.Count });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShowtimes()
        {
            var showtimes = await _db.Showtimes
                .Include(s => s.Room)
                .ToListAsync();

            var result = new List<object>();

            foreach (var s in showtimes)
            {
                var totalSeats = await _db.SeatShowtimes.CountAsync(x => x.ShowtimeId == s.Id);
                var occupiedSeats = await _db.SeatShowtimes.CountAsync(x => x.ShowtimeId == s.Id && x.Status != SeatStatus.Available);

                double occupancyRate = totalSeats == 0
                    ? 0
                    : Math.Round((double)occupiedSeats * 100 / totalSeats, 2);

                result.Add(new
                {
                    s.Id,
                    s.MovieId,
                    s.ShowDate,
                    s.FromTime,
                    s.ToTime,
                    Room = s.Room?.RoomName,
                    OccupancyRate = occupancyRate
                });
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShowtimeById(int id)
        {
            var showtime = await _db.Showtimes
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showtime == null)
                return NotFound("Không tìm thấy suất chiếu.");

            var totalSeats = await _db.SeatShowtimes.CountAsync(x => x.ShowtimeId == showtime.Id);
            var occupiedSeats = await _db.SeatShowtimes.CountAsync(x => x.ShowtimeId == showtime.Id && x.Status != SeatStatus.Available);

            double occupancyRate = totalSeats == 0
                ? 0
                : Math.Round((double)occupiedSeats * 100 / totalSeats, 2);

            var result = new
            {
                showtime.Id,
                showtime.MovieId,
                showtime.ShowDate,
                showtime.FromTime,
                showtime.ToTime,
                Room = showtime.Room?.RoomName,
                OccupancyRate = occupancyRate
            };

            return Ok(result);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> EditShowtime(int id, [FromBody] EditShowtimeDto dto)
        {
            var showtime = await _db.Showtimes.Include(s => s.Room).FirstOrDefaultAsync(s => s.Id == id);
            if (showtime == null) return NotFound();

            var hasBookings = await _db.OrderDetails.AnyAsync(od => od.ShowtimeId == id);

            if (hasBookings && dto.RoomId.HasValue && dto.RoomId != showtime.RoomId)
                return BadRequest("Không thể đổi phòng khi đã có vé được đặt.");

            if (dto.ShowDate.HasValue && dto.StartTime.HasValue)
            {
                var movie = await _movieService.GetMovieById(showtime.MovieId);
                if (movie == null || movie.Duration == null)
                    return BadRequest("Không thể xác minh thời lượng phim.");

                var newEnd = dto.StartTime.Value.AddMinutes(movie.Duration.Value + dto.BufferMinutes);

                var overlaps = await _db.Showtimes
                    .Where(s => s.Id != id &&
                                s.RoomId == (dto.RoomId ?? showtime.RoomId) &&
                                s.ShowDate == dto.ShowDate)
                    .ToListAsync();

                foreach (var s in overlaps)
                {
                    var sEnd = s.FromTime.AddMinutes(movie.Duration.Value + dto.BufferMinutes);
                    if (!(dto.StartTime.Value >= sEnd || newEnd <= s.FromTime))
                        return Conflict("Trùng giờ với suất chiếu khác.");
                }

                showtime.ShowDate = dto.ShowDate.Value;
                showtime.FromTime = dto.StartTime.Value;
            }

            if (dto.RoomId.HasValue && !hasBookings)
            {
                showtime.RoomId = dto.RoomId.Value;
                _db.SeatShowtimes.RemoveRange(_db.SeatShowtimes.Where(x => x.ShowtimeId == id));

                var newSeats = await _db.Seats.Where(s => s.RoomId == dto.RoomId.Value).ToListAsync();
                foreach (var seat in newSeats)
                {
                    _db.SeatShowtimes.Add(new SeatShowtime
                    {
                        ShowtimeId = showtime.Id,
                        SeatId = seat.Id,
                        Status = SeatStatus.Available
                    });
                }
            }

            await _db.SaveChangesAsync();
            return Ok(showtime);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelShowtime(int id)
        {
            var showtime = await _db.Showtimes.FirstOrDefaultAsync(s => s.Id == id);
            if (showtime == null) return NotFound();

            var orders = await _db.OrderDetails
                .Where(od => od.ShowtimeId == id)
                .Select(od => od.OrderId).Distinct().ToListAsync();

            if (orders.Any())
            {
                // TODO: Gọi BookingService để hoàn tiền và NotificationService để thông báo
                // --> Gợi ý: publish event qua RabbitMQ hoặc gửi HTTP
            }

            _db.SeatShowtimes.RemoveRange(_db.SeatShowtimes.Where(s => s.ShowtimeId == id));
            _db.Showtimes.Remove(showtime);
            await _db.SaveChangesAsync();

            return Ok("Showtime đã được hủy và dọn dẹp.");
        }
    }
}
