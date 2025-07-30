using BookingManagement.Controllers;
using BookingManagement.Models.DTOs;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CreateShowtimeDtoV1 = BookingManagement.Models.DTOs.CreateShowtimeDto;

namespace BookingManagement.Tests.Controllers
{
    public class ShowtimeControllerTest
    {
        private readonly Mock<IShowtimeService> _mockService;
        private readonly ShowtimeController _controller;

        public ShowtimeControllerTest()
        {
            _mockService = new Mock<IShowtimeService>();
            _controller = new ShowtimeController(_mockService.Object);
        }

        [Fact]
        // Kiểm tra khi gọi tạo lịch chiếu hợp lệ, controller trả về OkObjectResult
        public async Task CreateShowtime_ReturnsOkResult()
        {
            // Arrange
            var dto = new CreateShowtimeDtoV1
            {
                MovieId = 1,
                RoomId = 2,
                StartTime = TimeOnly.FromDateTime(DateTime.Now)
            };
            var expectedResult = new OkObjectResult(new { Message = "Showtime created" });

            _mockService.Setup(s => s.CreateShowtime(dto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateShowtime(dto);

            // Assert
            Assert.Same(expectedResult, result);
        }

        //[Fact]
        //// Kiểm tra khi gọi lấy danh sách lịch chiếu, controller trả về OkObjectResult
        //public async Task GetAllShowtimes_ReturnsOkResult()
        //{
        //    // Arrange
        //    var expectedResult = new OkObjectResult(new List<ShowtimeDto>());
        //    _mockService.Setup(s => s.GetAllShowtimes())
        //        .ReturnsAsync(expectedResult);

        //    // Act
        //    var result = await _controller.GetAllShowtimes();

        //    // Assert
        //    Assert.Same(expectedResult, result);
        //}

        //[Fact]
        //// Kiểm tra khi lấy lịch chiếu theo ID hợp lệ, controller trả về OkObjectResult chứa dữ liệu
        //public async Task GetShowtimeById_ExistingId_ReturnsOkResult()
        //{
        //    // Arrange
        //    int id = 5;
        //    var expectedResult = new OkObjectResult(new ShowtimeDto { Id = id });

        //    _mockService.Setup(s => s.GetShowtimeById(id))
        //        .ReturnsAsync(expectedResult);

        //    // Act
        //    var result = await _controller.GetShowtimeById(id);

        //    // Assert
        //    Assert.Same(expectedResult, result);
        //}

        [Fact]
        // Kiểm tra khi cập nhật lịch chiếu hợp lệ, controller trả về OkObjectResult
        public async Task EditShowtime_Valid_ReturnsOkResult()
        {
            // Arrange
            int id = 3;
            var dto = new EditShowtimeDto
            {
                RoomId = 2,
                StartTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(2))
            };
            var expectedResult = new OkObjectResult(new { Message = "Showtime updated" });

            _mockService.Setup(s => s.EditShowtime(id, dto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.EditShowtime(id, dto);

            // Assert
            Assert.Same(expectedResult, result);
        }

        [Fact]
        // Kiểm tra khi huỷ lịch chiếu hợp lệ, controller trả về OkObjectResult
        public async Task CancelShowtime_ValidId_ReturnsOkResult()
        {
            // Arrange
            int id = 7;
            var expectedResult = new OkObjectResult(new { Message = "Showtime cancelled" });

            _mockService.Setup(s => s.CancelShowtime(id))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CancelShowtime(id);

            // Assert
            Assert.Same(expectedResult, result);
        }
    }
}
