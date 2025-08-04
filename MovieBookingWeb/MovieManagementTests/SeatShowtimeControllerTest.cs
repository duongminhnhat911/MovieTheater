using BookingManagement.Controllers;
using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookingManagementTests
{
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
            var seats = new List<SeatShowtime> { new SeatShowtime { SeatId = 1, ShowtimeId = 1 } };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(seats);

            var result = await _controller.GetAll();

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
            _mockService
                .Setup(s => s.UpdateStatusAsync(1, 1, SeatStatus.Held))
                .ReturnsAsync((SeatShowtime?)null);

            var result = await _controller.Update(1, 1, SeatStatus.Held);

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
                    Status = SeatStatus.Available.ToString()
                }
            }
            };

            _mockService.Setup(s => s.GetSeatsByShowtimeAsync(5)).ReturnsAsync(dto);

            var result = await _controller.GetSeatsByShowtime(5);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
        }

        [Fact]
        // GetSeatsByShowtime(id) trả về NotFound khi không tìm thấy suất chiếu
        public async Task GetSeatsByShowtime_ReturnsNotFound_WhenNull()
        {
            _mockService
                .Setup(s => s.GetSeatsByShowtimeAsync(5))
                .Returns(Task.FromResult<SeatShowtimeDto?>(null));

            var result = await _controller.GetSeatsByShowtime(5);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Showtime không tồn tại.", notFound.Value);
        }

        /* -----Test luồng hoạt động------ */
        [Fact]
        // Test người dùng chọn ghế thành công (giữ ghế tạm thời - status = Held)
        public async Task HoldSeat_ReturnsOk_WhenSeatHeldSuccessfully()
        {
            var seatId = 4;
            var showtimeId = 2;
            var heldSeat = new SeatShowtime
            {
                SeatId = seatId,
                ShowtimeId = showtimeId,
                Status = SeatStatus.Held
            };

            _mockService
                .Setup(s => s.UpdateStatusAsync(seatId, showtimeId, SeatStatus.Held))
                .ReturnsAsync(heldSeat);

            var result = await _controller.Update(seatId, showtimeId, SeatStatus.Held);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(SeatStatus.Held, ((SeatShowtime)okResult.Value!).Status);
        }

        [Fact]
        // Test người dùng huỷ chọn ghế (trả ghế về trạng thái Available)
        public async Task CancelHeldSeat_ReturnsOk_WhenReleasedSuccessfully()
        {
            var seatId = 4;
            var showtimeId = 2;
            var seat = new SeatShowtime
            {
                SeatId = seatId,
                ShowtimeId = showtimeId,
                Status = SeatStatus.Available
            };

            _mockService
                .Setup(s => s.UpdateStatusAsync(seatId, showtimeId, SeatStatus.Available))
                .ReturnsAsync(seat);

            var result = await _controller.Update(seatId, showtimeId, SeatStatus.Available);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(SeatStatus.Available, ((SeatShowtime)okResult.Value!).Status);
        }

        [Fact]
        // Test người dùng chọn ghế đã bị người khác đặt (Booked) — nên trả về NotFound hoặc lỗi tùy xử lý
        public async Task HoldSeat_ReturnsNotFound_WhenSeatAlreadyBooked()
        {
            var seatId = 10;
            var showtimeId = 3;

            _mockService
                .Setup(s => s.UpdateStatusAsync(seatId, showtimeId, SeatStatus.Held))
                .ReturnsAsync((SeatShowtime?)null); // Giả định đã bị đặt

            var result = await _controller.Update(seatId, showtimeId, SeatStatus.Held);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Test lấy danh sách ghế cho UI đặt vé (có status rõ ràng cho từng ghế)
        public async Task GetSeatsByShowtime_ReturnsOk_WithVariousStatuses()
        {
            var dto = new SeatShowtimeDto
            {
                ShowtimeId = 6,
                Seats = new List<SeatDto>
                {
                    new SeatDto { SeatId = 1, Status = SeatStatus.Available.ToString() },
                    new SeatDto { SeatId = 2, Status = SeatStatus.Held.ToString() },
                    new SeatDto { SeatId = 3, Status = SeatStatus.Booked.ToString() }
                }
            };

            _mockService.Setup(s => s.GetSeatsByShowtimeAsync(6)).ReturnsAsync(dto);

            var result = await _controller.GetSeatsByShowtime(6);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<SeatShowtimeDto>(okResult.Value);
            Assert.Equal(3, returnedDto.Seats.Count);
            Assert.Contains(returnedDto.Seats, s => s.Status == SeatStatus.Held.ToString());
            Assert.Contains(returnedDto.Seats, s => s.Status == SeatStatus.Booked.ToString());
        }

        [Fact]
        // Test tạo ghế mới trong suất chiếu từ giao diện admin
        public async Task AdminCreateSeatShowtime_ReturnsOk_WhenCreated()
        {
            var newSeat = new SeatShowtime
            {
                SeatId = 11,
                ShowtimeId = 7,
                Status = SeatStatus.Available
            };

            _mockService.Setup(s => s.CreateAsync(newSeat)).ReturnsAsync(newSeat);

            var result = await _controller.Create(newSeat);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var created = Assert.IsType<SeatShowtime>(okResult.Value);
            Assert.Equal(11, created.SeatId);
        }
    }
}