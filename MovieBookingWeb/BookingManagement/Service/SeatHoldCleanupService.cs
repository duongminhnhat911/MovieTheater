using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using BookingManagement.Service;
using Microsoft.AspNetCore.SignalR;

namespace BookingManagement.BackgroundServices
{
    public class SeatHoldCleanupService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<SeatHoldCleanupService> _logger;

        public SeatHoldCleanupService(IServiceProvider provider, ILogger<SeatHoldCleanupService> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // ✅ BƯỚC 1: Xử lý ghế Held hết hạn ngay khi app khởi động
            using (var scope = _provider.CreateScope())
            {
                var seatService = scope.ServiceProvider.GetRequiredService<ISeatShowtimeService>();
                var hub = scope.ServiceProvider.GetRequiredService<IHubContext<SeatHub>>();

                var allSeats = await seatService.GetAllAsync();
                var expiredSeats = allSeats
                    .Where(s => s.Status == SeatStatus.Held && s.HeldUntil < DateTime.UtcNow)
                    .ToList();

                foreach (var seat in expiredSeats)
                {
                    seat.Status = SeatStatus.Available;
                    seat.HeldByUserId = null;
                    seat.HeldUntil = null;
                    await seatService.UpdateEntityAsync(seat);

                    await hub.Clients.Group($"showtime-{seat.ShowtimeId}")
                        .SendAsync("SeatUnlocked", seat.SeatId);
                }

                _logger.LogInformation($"🧹 Reset {expiredSeats.Count} held seats on startup.");
            }

            // ✅ BƯỚC 2: Tiếp tục chạy lặp mỗi 10s
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _provider.CreateScope())
                {
                    var seatService = scope.ServiceProvider.GetRequiredService<ISeatShowtimeService>();
                    var hub = scope.ServiceProvider.GetRequiredService<IHubContext<SeatHub>>();

                    var allSeats = await seatService.GetAllAsync();
                    var expiredSeats = allSeats
                        .Where(s => s.Status == SeatStatus.Held && s.HeldUntil < DateTime.UtcNow)
                        .ToList();

                    foreach (var seat in expiredSeats)
                    {
                        seat.Status = SeatStatus.Available;
                        seat.HeldByUserId = null;
                        seat.HeldUntil = null;
                        await seatService.UpdateEntityAsync(seat);

                        await hub.Clients.Group($"showtime-{seat.ShowtimeId}")
                            .SendAsync("SeatUnlocked", seat.SeatId);
                    }

                    _logger.LogInformation($"✅ Cleared {expiredSeats.Count} expired held seats at {DateTime.Now}");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}