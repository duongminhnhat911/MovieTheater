using MovieBookingWebMVC.Areas.Movie.Models;
namespace MovieBookingWebMVC.Areas.Movie.Services
{
    public interface IRoomService
    {
        Dictionary<string, Room> GetAllRoomLayouts();
        Room? GetRoomLayout(string roomName);
    }
}
