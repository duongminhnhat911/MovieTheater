using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Repositories;

namespace BookingManagement.Service
{
    // Services/SeatService.cs
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _repo;

        public SeatService(ISeatRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<object>> GetSeatsByRoomAsync(int roomId)
        {
            var seats = await _repo.GetSeatsByRoomAsync(roomId);
            return seats.Select(s => new
            {
                s.Id,
                s.SeatRow,
                s.SeatColumn,
                s.SeatStatus,
                RoomName = s.Room?.RoomName
            });
        }

        public async Task<Seat?> UpdateSeatAsync(int id, UpdateSeatDto dto)
        {
            var seat = await _repo.GetSeatByIdAsync(id);
            if (seat == null) return null;

            seat.SeatStatus = dto.SeatStatus ?? seat.SeatStatus;
            await _repo.SaveChangesAsync();
            return seat;
        }

        public async Task<object?> SoftDeleteSeatAsync(int id)
        {
            var seat = await _repo.GetSeatWithRoomAsync(id);
            if (seat == null) return null;

            seat.SeatStatus = false;
            await _repo.SaveChangesAsync();

            return new
            {
                Message = "Đã xóa mềm ghế (đã khóa).",
                SeatId = seat.Id,
                Room = seat.Room?.RoomName
            };
        }

        public async Task<bool> RestoreSeatAsync(int id)
        {
            var seat = await _repo.GetSeatByIdAsync(id);
            if (seat == null) return false;

            seat.SeatStatus = true;
            await _repo.SaveChangesAsync();
            return true;
        }
    }

}
