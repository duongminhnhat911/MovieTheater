using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public interface IRoomService
    {
        Task<List<RoomDto>> GetRoomsAsync();

        Task<RoomDto?> GetRoomByIdAsync(int id); // dùng cho hiển thị chi tiết

        Task<UpdateRoomDto?> GetRoomForEditAsync(int id); // dùng riêng cho edit

        Task<bool> CreateRoomAsync(CreateRoomDto dto);

        Task<bool> UpdateRoomAsync(int id, UpdateRoomDto dto);

        Task<List<SeatDto>> GetSeatsByRoomIdAsync(int roomId);
        Task<UpdateRoomViewModel?> GetRoomViewModelForEditAsync(int id);
        Task<bool> UpdateRoomFromViewModelAsync(UpdateRoomViewModel model);
        Task<RoomDetailsViewModel?> GetRoomDetailsAsync(int roomId);
    }
}
