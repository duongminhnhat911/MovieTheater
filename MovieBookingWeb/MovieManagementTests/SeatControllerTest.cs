using BookingManagement.Controllers;
using BookingManagement.Models.DTOs;
using BookingManagement.Service;
using BookingManagement.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace BookingManagement.Tests.Controllers
{
    public class SeatControllerTest
    {
        private readonly Mock<ISeatService> _mockService;
        private readonly SeatController _controller;

        public SeatControllerTest()
        {
            _mockService = new Mock<ISeatService>();
            _controller = new SeatController(_mockService.Object);
        }

        [Fact]
        // Đảm bảo GetSeatsByRoom trả về danh sách ghế đúng và mã trạng thái 200 (OK)
        public async Task GetSeatsByRoom_ShouldReturnOkWithData()
        {
            // Arrange
            int roomId = 1;
            var seats = new List<SeatDto>
            {
                new SeatDto { SeatId = 1, Row = 'A', Column = '1', Status = "Active" },
                new SeatDto { SeatId = 2, Row = 'A', Column = '2', Status = "Active" }
            };
            _mockService.Setup(s => s.GetSeatsByRoomAsync(roomId)).ReturnsAsync(seats);

            // Act
            var result = await _controller.GetSeatsByRoom(roomId);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(seats);
        }

        [Fact]
        // Đảm bảo UpdateSeat trả về NotFound khi không tìm thấy ghế cần cập nhật
        public async Task UpdateSeat_ShouldReturnNotFound_WhenSeatNotExists()
        {
            // Arrange
            int seatId = 99;
            var dto = new UpdateSeatDto { SeatStatus = false }; // Chỉ có duy nhất thuộc tính này

            _mockService
                .Setup(s => s.UpdateSeatAsync(seatId, dto))
                .ReturnsAsync((Seat?)null);

            // Act
            var result = await _controller.UpdateSeat(seatId, dto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        // Đảm bảo SoftDeleteSeat trả về ghế đã xóa và mã trạng thái OK khi thành công
        public async Task SoftDeleteSeat_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            int seatId = 1;
            var deletedSeat = new SeatDto { SeatId = seatId, Row = 'A', Column = '1', Status = "Deleted" };

            _mockService.Setup(s => s.SoftDeleteSeatAsync(seatId)).ReturnsAsync(deletedSeat);

            // Act
            var result = await _controller.SoftDeleteSeat(seatId);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(deletedSeat);
        }

        [Fact]
        // Đảm bảo SoftDeleteSeat trả về NotFound khi không tìm thấy ghế để xóa mềm
        public async Task SoftDeleteSeat_ShouldReturnNotFound_WhenSeatNotExists()
        {
            // Arrange
            int seatId = 999;
            _mockService.Setup(s => s.SoftDeleteSeatAsync(seatId)).ReturnsAsync((SeatDto?)null);

            // Act
            var result = await _controller.SoftDeleteSeat(seatId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        // Đảm bảo RestoreSeat trả về OK với thông báo khi khôi phục ghế thành công
        public async Task RestoreSeat_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            int seatId = 3;
            _mockService.Setup(s => s.RestoreSeatAsync(seatId)).ReturnsAsync(true);

            // Act
            var result = await _controller.RestoreSeat(seatId);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be("Đã khôi phục ghế.");
        }

        [Fact]
        // Đảm bảo RestoreSeat trả về NotFound khi ghế cần khôi phục không tồn tại
        public async Task RestoreSeat_ShouldReturnNotFound_WhenSeatNotExists()
        {
            // Arrange
            int seatId = 123;
            _mockService.Setup(s => s.RestoreSeatAsync(seatId)).ReturnsAsync(false);

            // Act
            var result = await _controller.RestoreSeat(seatId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
