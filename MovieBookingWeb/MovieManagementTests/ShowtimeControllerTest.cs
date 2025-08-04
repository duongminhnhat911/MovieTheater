using BookingManagement.Controllers;
using BookingManagement.Models.DTOs;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CreateShowtimeDtoV1 = BookingManagement.Models.DTOs.CreateShowtimeDto;

namespace BookingManagementTests
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
            var dto = new CreateShowtimeDtoV1
            {
                MovieId = 1,
                RoomId = 2,
                StartTime = TimeOnly.FromDateTime(DateTime.Now)
            };
            var expectedResult = new OkObjectResult(new { Message = "Showtime created" });

            _mockService.Setup(s => s.CreateShowtime(dto))
                .ReturnsAsync(expectedResult);

            var result = await _controller.CreateShowtime(dto);

            Assert.Same(expectedResult, result);
        }

        [Fact]
        // Kiểm tra khi cập nhật lịch chiếu hợp lệ, controller trả về OkObjectResult
        public async Task EditShowtime_Valid_ReturnsOkResult()
        {
            int id = 3;
            var dto = new EditShowtimeDto
            {
                RoomId = 2,
                StartTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(2))
            };
            var expectedResult = new OkObjectResult(new { Message = "Showtime updated" });

            _mockService.Setup(s => s.EditShowtime(id, dto))
                .ReturnsAsync(expectedResult);

            var result = await _controller.EditShowtime(id, dto);

            Assert.Same(expectedResult, result);
        }

        [Fact]
        // Kiểm tra khi huỷ lịch chiếu hợp lệ, controller trả về OkObjectResult
        public async Task CancelShowtime_ValidId_ReturnsOkResult()
        {
            int id = 7;
            var expectedResult = new OkObjectResult(new { Message = "Showtime cancelled" });

            _mockService.Setup(s => s.CancelShowtime(id))
                .ReturnsAsync(expectedResult);

            var result = await _controller.CancelShowtime(id);

            Assert.Same(expectedResult, result);
        }

        /* -----Test luồng hoạt động------ */
        [Fact]
        // Lấy danh sách tất cả lịch chiếu
        public async Task GetAllShowtimes_ReturnsOkWithShowtimes()
        {
            var mockShowtimes = new List<CreateShowtimeDtoV1>
            {
                new CreateShowtimeDtoV1
                {
                    MovieId = 10,
                    RoomId = 1,
                    ShowDate = DateOnly.FromDateTime(DateTime.Today),
                    StartTime = TimeOnly.Parse("10:00")
                },
                new CreateShowtimeDtoV1
                {
                    MovieId = 11,
                    RoomId = 2,
                    ShowDate = DateOnly.FromDateTime(DateTime.Today),
                    StartTime = TimeOnly.Parse("13:30")
                }
            };

            _mockService.Setup(s => s.GetAllShowtimes())
                .ReturnsAsync(new OkObjectResult(mockShowtimes));

            var result = await _controller.GetAllShowtimes();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CreateShowtimeDtoV1>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetShowtimeById_ValidId_ReturnsOkWithShowtime()
        {
            int showtimeId = 5;
            var showtime = new CreateShowtimeDtoV1
            {
                MovieId = 10,
                RoomId = 2,
                ShowDate = DateOnly.FromDateTime(DateTime.Today),
                StartTime = TimeOnly.Parse("14:00")
            };

            _mockService.Setup(s => s.GetShowtimeById(showtimeId))
                .ReturnsAsync(new OkObjectResult(showtime));

            var result = await _controller.GetShowtimeById(showtimeId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CreateShowtimeDtoV1>(okResult.Value);

            Assert.Equal(10, returnValue.MovieId);
            Assert.Equal(2, returnValue.RoomId);
            Assert.Equal(TimeOnly.Parse("14:00"), returnValue.StartTime);
        }

        [Fact]
        // Tạo lịch chiếu với dữ liệu không hợp lệ
        public async Task CreateShowtime_InvalidInput_ReturnsBadRequest()
        {
            var dto = new CreateShowtimeDtoV1
            {
                MovieId = 999, // giả lập không tồn tại
                RoomId = 2,
                StartTime = TimeOnly.Parse("00:00")
            };

            _mockService.Setup(s => s.CreateShowtime(dto))
                .ReturnsAsync(new BadRequestObjectResult(new { Error = "Invalid movie ID" }));

            var result = await _controller.CreateShowtime(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid movie ID", ((dynamic)badRequest.Value).Error);
        }

        [Fact]
        // Huỷ lịch chiếu với ID không tồn tại
        public async Task CancelShowtime_InvalidId_ReturnsNotFound()
        {
            int invalidId = 999;

            _mockService.Setup(s => s.CancelShowtime(invalidId))
                .ReturnsAsync(new NotFoundObjectResult(new { Error = "Showtime not found" }));

            var result = await _controller.CancelShowtime(invalidId);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Showtime not found", ((dynamic)notFound.Value).Error);
        }

        [Fact]
        // Tạo ➝ Lấy ➝ Cập nhật ➝ Huỷ (End-to-End)
        public async Task FullShowtimeFlow_CreateThenUpdateThenCancel()
        {
            var createDto = new CreateShowtimeDtoV1
            {
                MovieId = 1,
                RoomId = 1,
                StartTime = TimeOnly.Parse("09:00")
            };

            _mockService.Setup(s => s.CreateShowtime(createDto))
                .ReturnsAsync(new OkObjectResult(new { Message = "Showtime created" }));

            var createResult = await _controller.CreateShowtime(createDto);
            Assert.IsType<OkObjectResult>(createResult);

            var editDto = new EditShowtimeDto
            {
                RoomId = 2,
                StartTime = TimeOnly.Parse("10:30")
            };

            _mockService.Setup(s => s.EditShowtime(1, editDto))
                .ReturnsAsync(new OkObjectResult(new { Message = "Showtime updated" }));

            var editResult = await _controller.EditShowtime(1, editDto);
            Assert.IsType<OkObjectResult>(editResult);

            _mockService.Setup(s => s.CancelShowtime(1))
                .ReturnsAsync(new OkObjectResult(new { Message = "Showtime cancelled" }));

            var cancelResult = await _controller.CancelShowtime(1);
            Assert.IsType<OkObjectResult>(cancelResult);
        }
    }
}

