using Microsoft.AspNetCore.SignalR;
namespace BookingManagement.Models.Entities
{
    public class SeatHub : Hub
    {
        // Khi người dùng mở trang chọn ghế cho một suất chiếu
        public async Task JoinShowtimeGroup(int showtimeId)
        {
            try
            {
                Console.WriteLine($"🔗 Attempting to join showtime-{showtimeId}, Connection: {Context.ConnectionId}");
                await Groups.AddToGroupAsync(Context.ConnectionId, $"showtime-{showtimeId}");
                Console.WriteLine($"✅ Joined showtime-{showtimeId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in JoinShowtimeGroup: {ex.Message}");
                throw;
            }
        }

        // Khi người dùng rời trang chọn ghế
        public async Task LeaveShowtimeGroup(int showtimeId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"showtime-{showtimeId}");
        }
    }
}
