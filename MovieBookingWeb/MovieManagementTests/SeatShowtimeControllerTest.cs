using Moq;
using BookingManagement.Models.DTOs;
using BookingManagement.Controllers;
using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;

public class SeatShowtimeControllerTest
{
    private readonly Mock<ISeatShowtimeService> _mockService;
    private readonly SeatShowtimeController _controller;

    public SeatShowtimeControllerTest()
    {
        _mockService = new Mock<ISeatShowtimeService>();
        _controller = new SeatShowtimeController(_mockService.Object);
    }

    [Fact]
    // GetAll() trả về OkObjectResult chứa danh sách ghế theo suất chiếu
    public async Task GetAll_ReturnsOkResult_WithList()
    {
        // Arrange
        var seats = new List<SeatShowtime> { new SeatShowtime { SeatId = 1, ShowtimeId = 1 } };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(seats);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(seats, okResult.Value);
    }

    [Fact]
    // Get(id, id) trả về Ok khi tìm thấy ghế theo suất chiếu
    public async Task Get_ReturnsOk_WhenFound()
    {
        var seat = new SeatShowtime { SeatId = 1, ShowtimeId = 1 };
        _mockService.Setup(s => s.GetAsync(1, 1)).ReturnsAsync(seat);

        var result = await _controller.Get(1, 1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(seat, okResult.Value);
    }

    [Fact]
    // Get(id, id) trả về NotFound khi không tìm thấy ghế
    public async Task Get_ReturnsNotFound_WhenNull()
    {
        _mockService.Setup(s => s.GetAsync(1, 1)).ReturnsAsync((SeatShowtime)null);

        var result = await _controller.Get(1, 1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Create() trả về Ok với đối tượng SeatShowtime vừa tạo
    public async Task Create_ReturnsOk_WithCreatedSeat()
    {
        var seat = new SeatShowtime { SeatId = 2, ShowtimeId = 3 };
        _mockService.Setup(s => s.CreateAsync(seat)).ReturnsAsync(seat);

        var result = await _controller.Create(seat);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(seat, okResult.Value);
    }

    [Fact]
    // Update() trả về Ok khi cập nhật trạng thái thành công
    public async Task Update_ReturnsOk_WhenUpdated()
    {
        var updatedSeat = new SeatShowtime { SeatId = 1, ShowtimeId = 1, Status = SeatStatus.Booked };
        _mockService.Setup(s => s.UpdateStatusAsync(1, 1, SeatStatus.Booked)).ReturnsAsync(updatedSeat);

        var result = await _controller.Update(1, 1, SeatStatus.Booked);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updatedSeat, okResult.Value);
    }

    [Fact]
    // Update() trả về NotFound khi ghế không tồn tại hoặc cập nhật thất bại
    public async Task Update_ReturnsNotFound_WhenNotFound()
    {
        // Arrange
        _mockService
            .Setup(s => s.UpdateStatusAsync(1, 1, SeatStatus.Held)) // match enum sau khi controller parse từ "Held"
            .ReturnsAsync((SeatShowtime?)null);

        // Act
        var result = await _controller.Update(1, 1, SeatStatus.Held); // truyền string đúng như controller yêu cầu

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Delete() trả về Ok khi xoá ghế thành công
    public async Task Delete_ReturnsOk_WhenDeleted()
    {
        _mockService.Setup(s => s.DeleteAsync(1, 1)).ReturnsAsync(true);

        var result = await _controller.Delete(1, 1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Đã xoá thành công.", okResult.Value);
    }

    [Fact]
    // Delete() trả về NotFound khi xoá thất bại (ghế không tồn tại)
    public async Task Delete_ReturnsNotFound_WhenFail()
    {
        _mockService.Setup(s => s.DeleteAsync(1, 1)).ReturnsAsync(false);

        var result = await _controller.Delete(1, 1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // GetSeatsByShowtime(id) trả về danh sách ghế của suất chiếu khi tồn tại
    public async Task GetSeatsByShowtime_ReturnsOk_WhenFound()
    {
        // Arrange
        var dto = new SeatShowtimeDto
        {
            ShowtimeId = 5,
            Seats = new List<SeatDto> {
                new SeatDto {
                    SeatId = 1,
                    Status = SeatStatus.Available.ToString() // chuyển enum sang string
                }
            }
        };

        _mockService.Setup(s => s.GetSeatsByShowtimeAsync(5)).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetSeatsByShowtime(5);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    // GetSeatsByShowtime(id) trả về NotFound khi không tìm thấy suất chiếu
    public async Task GetSeatsByShowtime_ReturnsNotFound_WhenNull()
    {
        // Arrange: giả lập trả về null
        _mockService
            .Setup(s => s.GetSeatsByShowtimeAsync(5))
            .Returns(Task.FromResult<SeatShowtimeDto?>(null));

        // Act
        var result = await _controller.GetSeatsByShowtime(5);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Showtime không tồn tại.", notFound.Value);
    }
}
