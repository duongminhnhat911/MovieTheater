using BookingManagement.Models.Entities.Enums;
using BookingManagement.Models.Entities;
using BookingManagement.Repositories;
using BookingManagement.Models.DTOs;

namespace BookingManagement.Service
{
    // Services/SeatShowtimeService.cs
    public class SeatShowtimeService : ISeatShowtimeService
    {
        private readonly ISeatShowtimeRepository _repo;
        private readonly IShowtimeRepository _showtimeRepository;
        private readonly ISeatRepository _seatRepository;

        public SeatShowtimeService(ISeatShowtimeRepository repo, IShowtimeRepository showtimeRepository, ISeatRepository seatRepository )
        {
            _repo = repo;
            _showtimeRepository = showtimeRepository;
            _seatRepository = seatRepository;
        }

        public async Task<List<SeatShowtime>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<SeatShowtime?> GetAsync(int showtimeId, int seatId) =>
            await _repo.GetAsync(showtimeId, seatId);

        public async Task<SeatShowtime> CreateAsync(SeatShowtime seatShowtime)
        {
            await _repo.AddAsync(seatShowtime);
            await _repo.SaveChangesAsync();
            return seatShowtime;
        }

        public async Task<SeatShowtime?> UpdateStatusAsync(int showtimeId, int seatId, SeatStatus status)
        {
            var entity = await _repo.GetAsync(showtimeId, seatId);
            if (entity == null) return null;

            entity.Status = status;
            await _repo.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int showtimeId, int seatId)
        {
            var entity = await _repo.GetAsync(showtimeId, seatId);
            if (entity == null) return false;

            _repo.Remove(entity);
            await _repo.SaveChangesAsync();
            return true;
        }
        public async Task<SeatShowtimeDto?> GetSeatsByShowtimeAsync(int showtimeId)
        {
            var showtime = await _showtimeRepository.GetShowtimeByIdAsync(showtimeId);
            if (showtime == null)
                return null;

            var seats = await _seatRepository.GetSeatsByRoomIdAsync(showtime.RoomId);
            var seatStatuses = await _repo.GetSeatStatusesAsync(showtimeId);

            var seatDtos = seats.Select(s => new SeatDto
            {
                SeatId = s.Id,
                Row = s.SeatRow,
                Column = s.SeatColumn,
                Status = seatStatuses.TryGetValue(s.Id, out var status)
                    ? status.ToString()
                    : "Unavailable"
            }).ToList();

            return new SeatShowtimeDto
            {
                RoomId = showtime.RoomId,
                ShowtimeId = showtimeId,
                Seats = seatDtos
            };
        }
    }
}
