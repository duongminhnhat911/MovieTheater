using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities.Enums;
using BookingManagement.Models.Entities;
using BookingManagement.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.Service
{
    // Services/ShowtimeService.cs
    public class ShowtimeService : IShowtimeService
    {
        private readonly IShowtimeRepository _repo;

        public ShowtimeService(IShowtimeRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> CreateShowtime(CreateShowtimeDto dto)
        {
            var room = await _repo.GetActiveRoomAsync(dto.RoomId);
            if (room == null) return new BadRequestObjectResult("Phòng không tồn tại hoặc đã bị khóa.");

            var movie = await _repo.GetMovieByIdAsync(dto.MovieId);
            if (movie == null || movie.Status != "Active") return new BadRequestObjectResult("Phim không tồn tại hoặc đã ngừng chiếu.");
            if (movie.Duration == null) return new BadRequestObjectResult("Phim chưa có thời lượng.");

            const int buffer = 15;
            var startTime = dto.StartTime;
            var endTime = startTime.AddMinutes(movie.Duration.Value + buffer);

            if (endTime > new TimeOnly(23, 59))
                return new BadRequestObjectResult("Suất chiếu kết thúc sau giờ hoạt động của rạp.");

            var overlapping = await _repo.GetOverlappingShowtimesAsync(dto.RoomId, dto.ShowDate);
            foreach (var s in overlapping)
            {
                if (!(startTime >= s.ToTime || endTime <= s.FromTime))
                    return new ConflictObjectResult("Suất chiếu bị trùng với lịch đã có.");
            }

            var showtime = new Showtime
            {
                MovieId = dto.MovieId,
                RoomId = dto.RoomId,
                ShowDate = dto.ShowDate,
                FromTime = startTime,
                ToTime = endTime
            };

            _repo.AddShowtime(showtime);
            await _repo.SaveChangesAsync();

            var seats = await _repo.GetSeatsByRoomAsync(dto.RoomId);
            foreach (var seat in seats)
            {
                _repo.AddSeatShowtime(new SeatShowtime
                {
                    ShowtimeId = showtime.Id,
                    SeatId = seat.Id,
                    Status = SeatStatus.Available
                });
            }

            await _repo.SaveChangesAsync();
            return new OkObjectResult("Suất chiếu được tạo thành công");
        }

        public async Task<IActionResult> GetAllShowtimes()
        {
            var showtimes = await _repo.GetAllShowtimesAsync();
            var result = new List<object>();

            foreach (var s in showtimes)
            {
                int total = await _repo.CountTotalSeatsAsync(s.Id);
                int booked = await _repo.CountOccupiedSeatsAsync(s.Id);
                double rate = total == 0 ? 0 : Math.Round((double)booked * 100 / total, 2);

                result.Add(new
                {
                    s.Id,
                    s.MovieId,
                    s.ShowDate,
                    s.FromTime,
                    s.ToTime,
                    Room = s.Room?.RoomName,
                });
            }

            return new OkObjectResult(result);
        }

        public async Task<IActionResult> GetShowtimeById(int id)
        {
            var s = await _repo.GetShowtimeByIdAsync(id);
            if (s == null) return new NotFoundObjectResult("Không tìm thấy suất chiếu.");

            int total = await _repo.CountTotalSeatsAsync(s.Id);
            int booked = await _repo.CountOccupiedSeatsAsync(s.Id);
            double rate = total == 0 ? 0 : Math.Round((double)booked * 100 / total, 2);

            var result = new
            {
                s.Id,
                s.MovieId,
                s.ShowDate,
                s.FromTime,
                s.ToTime,
                Room = s.Room?.RoomName
            };

            return new OkObjectResult(result);
        }

        public async Task<IActionResult> EditShowtime(int id, EditShowtimeDto dto)
        {
            var s = await _repo.GetShowtimeByIdAsync(id);
            if (s == null) return new NotFoundResult();

            var hasBooking = (await _repo.GetOrderDetailsByShowtimeIdAsync(id)).Any();

            if (hasBooking && dto.RoomId.HasValue && dto.RoomId != s.RoomId)
                return new BadRequestObjectResult("Không thể đổi phòng khi đã có vé được đặt.");

            if (dto.ShowDate.HasValue && dto.StartTime.HasValue)
            {
                var movie = await _repo.GetMovieByIdAsync(s.MovieId);
                if (movie?.Duration == null)
                    return new BadRequestObjectResult("Không thể xác minh thời lượng phim.");

                var newEnd = dto.StartTime.Value.AddMinutes(movie.Duration.Value + dto.BufferMinutes);

                var overlaps = await _repo.GetOverlappingShowtimesAsync(dto.RoomId ?? s.RoomId, dto.ShowDate.Value);
                foreach (var other in overlaps.Where(x => x.Id != id))
                {
                    var end = other.FromTime.AddMinutes(movie.Duration.Value + dto.BufferMinutes);
                    if (!(dto.StartTime.Value >= end || newEnd <= other.FromTime))
                        return new ConflictObjectResult("Trùng giờ với suất chiếu khác.");
                }

                s.ShowDate = dto.ShowDate.Value;
                s.FromTime = dto.StartTime.Value;
            }

            if (dto.RoomId.HasValue && !hasBooking)
            {
                s.RoomId = dto.RoomId.Value;
                _repo.RemoveSeatShowtimes(id);
                var seats = await _repo.GetSeatsByRoomAsync(dto.RoomId.Value);
                foreach (var seat in seats)
                {
                    _repo.AddSeatShowtime(new SeatShowtime
                    {
                        ShowtimeId = id,
                        SeatId = seat.Id,
                        Status = SeatStatus.Available
                    });
                }
            }

            await _repo.SaveChangesAsync();
            return new OkObjectResult(s);
        }

        public async Task<IActionResult> CancelShowtime(int id)
        {
            var s = await _repo.GetShowtimeByIdAsync(id);
            if (s == null) return new NotFoundResult();

            var orderIds = await _repo.GetOrderIdsByShowtimeIdAsync(id);
            if (orderIds.Any())
            {
                // TODO: Hoàn tiền, gửi thông báo
            }

            _repo.RemoveSeatShowtimes(id);
            _repo.RemoveShowtime(s);
            await _repo.SaveChangesAsync();

            return new OkObjectResult("Showtime đã được hủy và dọn dẹp.");
        }
    }
}
