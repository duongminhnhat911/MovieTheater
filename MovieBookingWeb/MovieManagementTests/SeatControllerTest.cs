using BookingManagement.Controllers;
using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Service;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookingManagementTests
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
            int roomId = 1;
            var seats = new List<SeatDto>
        {
            new SeatDto { SeatId = 1, Row = 'A', Column = '1', Status = "Active" },
            new SeatDto { SeatId = 2, Row = 'A', Column = '2', Status = "Active" }
        };
            _mockService.Setup(s => s.GetSeatsByRoomAsync(roomId)).ReturnsAsync(seats);

            var result = await _controller.GetSeatsByRoom(roomId);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(seats);
        }

        [Fact]
        // Đảm bảo UpdateSeat trả về NotFound khi không tìm thấy ghế cần cập nhật
        public async Task UpdateSeat_ShouldReturnNotFound_WhenSeatNotExists()
        {
            int seatId = 99;
            var dto = new UpdateSeatDto { SeatStatus = false };

            _mockService
                .Setup(s => s.UpdateSeatAsync(seatId, dto))
                .ReturnsAsync((Seat?)null);

            var result = await _controller.UpdateSeat(seatId, dto);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        // Đảm bảo SoftDeleteSeat trả về ghế đã xóa và mã trạng thái OK khi thành công
        public async Task SoftDeleteSeat_ShouldReturnOk_WhenSuccessful()
        {
            int seatId = 1;
            var deletedSeat = new SeatDto { SeatId = seatId, Row = 'A', Column = '1', Status = "Deleted" };

            _mockService.Setup(s => s.SoftDeleteSeatAsync(seatId)).ReturnsAsync(deletedSeat);

            var result = await _controller.SoftDeleteSeat(seatId);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(deletedSeat);
        }

        [Fact]
        // Đảm bảo SoftDeleteSeat trả về NotFound khi không tìm thấy ghế để xóa mềm
        public async Task SoftDeleteSeat_ShouldReturnNotFound_WhenSeatNotExists()
        {
            int seatId = 999;
            _mockService.Setup(s => s.SoftDeleteSeatAsync(seatId)).ReturnsAsync((SeatDto?)null);

            var result = await _controller.SoftDeleteSeat(seatId);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        // Đảm bảo RestoreSeat trả về OK với thông báo khi khôi phục ghế thành công
        public async Task RestoreSeat_ShouldReturnOk_WhenSuccessful()
        {
            int seatId = 3;
            _mockService.Setup(s => s.RestoreSeatAsync(seatId)).ReturnsAsync(true);

            var result = await _controller.RestoreSeat(seatId);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be("Đã khôi phục ghế.");
        }

        [Fact]
        // Đảm bảo RestoreSeat trả về NotFound khi ghế cần khôi phục không tồn tại
        public async Task RestoreSeat_ShouldReturnNotFound_WhenSeatNotExists()
        {
            int seatId = 123;
            _mockService.Setup(s => s.RestoreSeatAsync(seatId)).ReturnsAsync(false);

            var result = await _controller.RestoreSeat(seatId);

            result.Should().BeOfType<NotFoundResult>();
        }

        /* -----Test luồng hoạt động------ */
        [Fact]
        // Lấy danh sách ghế, rồi cập nhật trạng thái một ghế
        public async Task UserFlow_ViewSeatsThenUpdateOneSeat_ShouldReflectUpdate()
        {
            int roomId = 1;
            int seatIdToUpdate = 2;
            var initialSeats = new List<SeatDto>
            {
                new SeatDto { SeatId = 1, Row = 'A', Column = '1', Status = "Active" },
                new SeatDto { SeatId = 2, Row = 'A', Column = '2', Status = "Active" }
            };

            var updatedSeat = new Seat
            {
                Id = seatIdToUpdate,
                SeatRow = 'A',
                SeatColumn = '2',
                SeatStatus = false
            };

            _mockService.Setup(s => s.GetSeatsByRoomAsync(roomId)).ReturnsAsync(initialSeats);
            _mockService.Setup(s => s.UpdateSeatAsync(seatIdToUpdate, It.IsAny<UpdateSeatDto>())).ReturnsAsync(updatedSeat);

            var resultBefore = await _controller.GetSeatsByRoom(roomId);
            var updateResult = await _controller.UpdateSeat(seatIdToUpdate, new UpdateSeatDto { SeatStatus = false });

            var beforeOk = resultBefore as OkObjectResult;
            beforeOk.Should().NotBeNull();
            var seatsBefore = beforeOk!.Value as List<SeatDto>;
            seatsBefore!.First(s => s.SeatId == seatIdToUpdate).Status.Should().Be("Active");

            var updatedOk = updateResult as OkObjectResult;
            updatedOk.Should().NotBeNull();
            var updated = updatedOk!.Value as Seat;
            updatedSeat.SeatStatus.Should().BeFalse();
        }

        [Fact]
        // Xóa mềm ghế rồi cố gắng khôi phục lại
        public async Task UserFlow_DeleteThenRestoreSeat_ShouldSucceed()
        {
            int seatId = 5;

            var deletedSeat = new SeatDto
            {
                SeatId = seatId,
                Row = 'B',
                Column = '4',
                Status = "Deleted"
            };

            _mockService.Setup(s => s.SoftDeleteSeatAsync(seatId)).ReturnsAsync(deletedSeat);
            _mockService.Setup(s => s.RestoreSeatAsync(seatId)).ReturnsAsync(true);

            var deleteResult = await _controller.SoftDeleteSeat(seatId);
            var restoreResult = await _controller.RestoreSeat(seatId);

            deleteResult.Should().BeOfType<OkObjectResult>();
            restoreResult.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)restoreResult).Value.Should().Be("Đã khôi phục ghế.");
        }

        [Fact]
        // Khôi phục ghế không tồn tại
        public async Task UserFlow_RestoreSeatNotExist_ShouldReturnNotFound()
        {
            int seatId = 999;
            _mockService.Setup(s => s.RestoreSeatAsync(seatId)).ReturnsAsync(false);

            var result = await _controller.RestoreSeat(seatId);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        // Hiển thị danh sách ghế sau khi khôi phục
        public async Task UserFlow_ViewSeatsAfterRestore_ShouldIncludeRestoredSeat()
        {
            int roomId = 2;
            int seatId = 8;

            var seatsAfterRestore = new List<SeatDto>
            {
                new SeatDto { SeatId = 8, Row = 'C', Column = '3', Status = "Active" }
            };

            _mockService.Setup(s => s.RestoreSeatAsync(seatId)).ReturnsAsync(true);
            _mockService.Setup(s => s.GetSeatsByRoomAsync(roomId)).ReturnsAsync(seatsAfterRestore);

            var restoreResult = await _controller.RestoreSeat(seatId);
            var result = await _controller.GetSeatsByRoom(roomId);

            restoreResult.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            var seats = okResult!.Value as List<SeatDto>;
            seats.Should().Contain(s => s.SeatId == seatId && s.Status == "Active");
        }
    }
}
