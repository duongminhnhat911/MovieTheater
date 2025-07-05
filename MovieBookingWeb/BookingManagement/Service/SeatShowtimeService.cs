using BookingManagement.Models.Entities.Enums;
using BookingManagement.Models.Entities;
using BookingManagement.Repositories;

namespace BookingManagement.Service
{
    // Services/SeatShowtimeService.cs
    public class SeatShowtimeService : ISeatShowtimeService
    {
        private readonly ISeatShowtimeRepository _repo;

        public SeatShowtimeService(ISeatShowtimeRepository repo)
        {
            _repo = repo;
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
    }
}
