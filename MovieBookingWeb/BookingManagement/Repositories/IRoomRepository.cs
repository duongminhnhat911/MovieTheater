using BookingManagement.Models.Entities;

namespace BookingManagement.Repositories
{
    public interface IRoomRepository
    {
        Task AddRoomAsync(Room room);
        Task<Room?> GetRoomByIdAsync(int id);
        Task<List<Room>> GetAllRoomsAsync();
        Task SaveChangesAsync();

        Task AddSeatAsync(Seat seat);
        Task<List<Showtime>> GetShowtimesByRoomIdAsync(int roomId);
        Task<int> CountSeatShowtimesAsync(int showtimeId);
        Task<int> CountBookedSeatShowtimesAsync(int showtimeId);
    }
}
