using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;

namespace BookingManagement.Service
{
    public interface IRoomService
    {
        Task<object> CreateRoomAsync(CreateRoomDto dto);
        Task<Room?> UpdateRoomAsync(int id, UpdateRoomDto dto);
        Task<List<object>> GetRoomUtilizationAsync(int id);
        Task<List<Room>> GetRoomsAsync();
        Task<Room?> GetRoomByIdAsync(int id);
    }
}
