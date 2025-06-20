using MovieBookingWeb.Models;
namespace MovieBookingWeb.Services
{
    public interface IRoomService
    {
        Dictionary<string, Room> GetAllRoomLayouts();
        Room? GetRoomLayout(string roomName);
    }
}