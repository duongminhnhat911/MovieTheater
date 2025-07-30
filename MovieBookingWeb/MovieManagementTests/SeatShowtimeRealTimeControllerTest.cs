using BookingManagement.Controllers;
using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;

public class SeatShowtimeRealTimeControllerTest
{
    private readonly Mock<ISeatShowtimeService> _mockService;
    private readonly Mock<IHubContext<SeatHub>> _mockHubContext;
    private readonly Mock<IClientProxy> _mockClientProxy;
    private readonly Mock<IHubClients> _mockClients;
    private readonly SeatShowtimeRealTimeController _controller;

    public SeatShowtimeRealTimeControllerTest()
    {
        _mockService = new Mock<ISeatShowtimeService>();
        _mockHubContext = new Mock<IHubContext<SeatHub>>();
        _mockClients = new Mock<IHubClients>();
        _mockClientProxy = new Mock<IClientProxy>();

        _mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);
        _mockHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);

        _controller = new SeatShowtimeRealTimeController(_mockService.Object, _mockHubContext.Object);
    }

    [Fact]
    // LockSeat trả về NotFound khi ghế không tồn tại
    public async Task LockSeat_ReturnsNotFound_WhenSeatIsNull()
    {
        var dto = new LockSeatShowtimeRequest { ShowtimeId = 1, SeatId = 1, UserId = 99 };
        _mockService.Setup(s => s.GetAsync(dto.ShowtimeId, dto.SeatId)).ReturnsAsync((SeatShowtime?)null);

        var result = await _controller.LockSeat(dto);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Ghế không tồn tại.", notFoundResult.Value);
    }

    [Fact]
    // LockSeat trả về Conflict nếu ghế không ở trạng thái Available
    public async Task LockSeat_ReturnsConflict_WhenSeatNotAvailable()
    {
        var dto = new LockSeatShowtimeRequest { ShowtimeId = 1, SeatId = 2, UserId = 99 };
        var seat = new SeatShowtime { SeatId = 2, ShowtimeId = 1, Status = SeatStatus.Held };
        _mockService.Setup(s => s.GetAsync(dto.ShowtimeId, dto.SeatId)).ReturnsAsync(seat);

        var result = await _controller.LockSeat(dto);

        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal("Ghế đã bị giữ hoặc đặt.", conflictResult.Value);
    }

    [Fact]
    // LockSeat thành công khi ghế đang ở trạng thái Available (cập nhật và phát SignalR)
    public async Task LockSeat_UpdatesSeatAndBroadcasts_WhenSeatIsAvailable()
    {
        var dto = new LockSeatShowtimeRequest { ShowtimeId = 1, SeatId = 3, UserId = 10 };
        var seat = new SeatShowtime
        {
            SeatId = 3,
            ShowtimeId = 1,
            Status = SeatStatus.Available
        };

        _mockService.Setup(s => s.GetAsync(dto.ShowtimeId, dto.SeatId)).ReturnsAsync(seat);
        _mockService.Setup(s => s.UpdateEntityAsync(It.IsAny<SeatShowtime>())).Returns(Task.CompletedTask);
        _mockClientProxy.Setup(c => c.SendCoreAsync("SeatLocked", It.IsAny<object[]>(), default))
                        .Returns(Task.CompletedTask);

        var result = await _controller.LockSeat(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Đã giữ ghế trong 3 phút.", okResult.Value);
        Assert.Equal(SeatStatus.Held, seat.Status);
        Assert.Equal(dto.UserId, seat.HeldByUserId);
        Assert.True(seat.HeldUntil > DateTime.UtcNow);
    }

    [Fact]
    // UnlockSeat trả về NotFound nếu ghế không tồn tại
    public async Task UnlockSeat_ReturnsNotFound_WhenSeatIsNull()
    {
        var dto = new LockSeatShowtimeRequest { ShowtimeId = 1, SeatId = 4, UserId = 5 };
        _mockService.Setup(s => s.GetAsync(dto.ShowtimeId, dto.SeatId)).ReturnsAsync((SeatShowtime?)null);

        var result = await _controller.UnlockSeat(dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // UnlockSeat thành công nếu đúng user đang giữ ghế
    public async Task UnlockSeat_SuccessfullyUnlocks_WhenSeatHeldBySameUser()
    {
        var dto = new LockSeatShowtimeRequest { ShowtimeId = 1, SeatId = 5, UserId = 999 };
        var seat = new SeatShowtime
        {
            SeatId = 5,
            ShowtimeId = 1,
            Status = SeatStatus.Held,
            HeldByUserId = 999,
            HeldUntil = DateTime.UtcNow.AddMinutes(2)
        };

        _mockService.Setup(s => s.GetAsync(dto.ShowtimeId, dto.SeatId)).ReturnsAsync(seat);
        _mockService.Setup(s => s.UpdateEntityAsync(It.IsAny<SeatShowtime>())).Returns(Task.CompletedTask);
        _mockClientProxy.Setup(c => c.SendCoreAsync("SeatUnlocked", It.IsAny<object[]>(), default))
                        .Returns(Task.CompletedTask);

        var result = await _controller.UnlockSeat(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Đã mở khóa ghế.", okResult.Value);
        Assert.Equal(SeatStatus.Available, seat.Status);
        Assert.Null(seat.HeldByUserId);
        Assert.Null(seat.HeldUntil);
    }

    [Fact]
    // UnlockSeat trả về BadRequest nếu user khác cố gắng mở khóa ghế
    public async Task UnlockSeat_ReturnsBadRequest_WhenSeatHeldByDifferentUser()
    {
        var dto = new LockSeatShowtimeRequest { ShowtimeId = 1, SeatId = 6, UserId = 123 };
        var seat = new SeatShowtime
        {
            SeatId = 6,
            ShowtimeId = 1,
            Status = SeatStatus.Held,
            HeldByUserId = 456
        };

        _mockService.Setup(s => s.GetAsync(dto.ShowtimeId, dto.SeatId)).ReturnsAsync(seat);

        var result = await _controller.UnlockSeat(dto);

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Không có quyền mở khóa hoặc trạng thái không hợp lệ.", badResult.Value);
    }
}
