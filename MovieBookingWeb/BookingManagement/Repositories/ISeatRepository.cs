using System.Threading.Tasks;
using BookingManagement.Models.Entities;

namespace BookingManagement.Repositories
{
    public interface ISeatRepository
    {
        Task<List<Seat>> GetSeatsByRoomAsync(int roomId);
        Task<Seat?> GetSeatByIdAsync(int id);
        Task<Seat?> GetSeatWithRoomAsync(int id);
        Task SaveChangesAsync();
        Task<List<string>> GetSeatNamesAsync(List<int> seatIds);

        Task<List<Seat>> GetSeatsByRoomIdAsync(int roomId);
    }
}
