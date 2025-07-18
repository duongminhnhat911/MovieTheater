using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;

namespace BookingManagement.Service
{
    public interface ISeatService
    {
        Task<IEnumerable<object>> GetSeatsByRoomAsync(int roomId);
        Task<Seat?> UpdateSeatAsync(int id, UpdateSeatDto dto);
        Task<object?> SoftDeleteSeatAsync(int id);
        Task<bool> RestoreSeatAsync(int id);
    }
}
