using BookingManagement.Controllers;
using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookingManagementTests
{
    public class OrderDetailControllerTest
    {
        private readonly Mock<IOrderDetailService> _mockService;
        private readonly OrderDetailController _controller;

        public OrderDetailControllerTest()
        {
            _mockService = new Mock<IOrderDetailService>();
            _controller = new OrderDetailController(_mockService.Object);
        }

        [Fact]
        // Kiểm tra tạo OrderDetail thành công trả về ID
        public async Task CreateOrderDetail_ReturnsOk_WithId()
        {
            var dto = new CreateOrderDetailDto { OrderId = 1, SeatId = 5 };
            var createdDetail = new OrderDetail { Id = 100, OrderId = 1, SeatId = 5 };
            _mockService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(createdDetail);

            var result = await _controller.CreateOrderDetail(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;

            var idProperty = value?.GetType().GetProperty("Id")?.GetValue(value, null);
            Assert.NotNull(idProperty);
            Assert.Equal(100, (int)idProperty);
        }

        [Fact]
        // Kiểm tra lấy danh sách OrderDetail trả về đúng kiểu và số lượng
        public async Task GetAllOrderDetails_ReturnsOk_WithList()
        {
            var details = new List<OrderDetail> {
            new OrderDetail { Id = 1 }, new OrderDetail { Id = 2 }
        };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(details);

            var result = await _controller.GetAllOrderDetails();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<List<OrderDetail>>(okResult.Value);
            Assert.Equal(2, returned.Count);
        }

        [Fact]
        // Kiểm tra lấy chi tiết đơn hàng theo ID hợp lệ
        public async Task GetOrderDetailById_ExistingId_ReturnsOk()
        {
            var detail = new OrderDetail { Id = 5 };
            _mockService.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(detail);

            var result = await _controller.GetOrderDetailById(5);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<OrderDetail>(okResult.Value);
            Assert.Equal(5, returned.Id);
        }

        [Fact]
        // Kiểm tra controller trả về NotFound khi không có OrderDetail
        public async Task GetOrderDetailById_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((OrderDetail?)null);

            var result = await _controller.GetOrderDetailById(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Không tìm thấy chi tiết đơn hàng.", notFound.Value);
        }

        [Fact]
        // Kiểm tra cập nhật OrderDetail thành công
        public async Task UpdateOrderDetail_ExistingId_ReturnsOk()
        {
            var dto = new UpdateOrderDetailDto { SeatId = 10 };
            var updated = new OrderDetail { Id = 1, SeatId = 10 };
            _mockService.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(updated);

            var result = await _controller.UpdateOrderDetail(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<OrderDetail>(okResult.Value);
            Assert.Equal(10, returned.SeatId);
        }

        [Fact]
        // Kiểm tra cập nhật thất bại nếu không tìm thấy OrderDetail
        public async Task UpdateOrderDetail_NotFound_ReturnsNotFound()
        {
            var dto = new UpdateOrderDetailDto { SeatId = 10 };
            _mockService.Setup(s => s.UpdateAsync(99, dto)).ReturnsAsync((OrderDetail?)null);

            var result = await _controller.UpdateOrderDetail(99, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Kiểm tra xóa OrderDetail thành công
        public async Task DeleteOrderDetail_ExistingId_ReturnsOk()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteOrderDetail(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Đã xóa chi tiết đơn hàng.", okResult.Value);
        }

        [Fact]
        // Kiểm tra xóa OrderDetail không tồn tại sẽ trả về NotFound
        public async Task DeleteOrderDetail_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.DeleteAsync(123)).ReturnsAsync(false);

            var result = await _controller.DeleteOrderDetail(123);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Kiểm tra lấy chi tiết đơn hàng đầy đủ theo OrderId thành công
        public async Task GetFullOrderDetail_ExistingOrderId_ReturnsOk()
        {
            var fullDto = new OrderDetailDto { OrderId = 1, TotalPrice = 200000 };
            _mockService.Setup(s => s.GetFullOrderDetailAsync(1)).ReturnsAsync(fullDto);

            var result = await _controller.GetFullOrderDetail(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<OrderDetailDto>(okResult.Value);
            Assert.Equal(1, returned.OrderId);
        }

        [Fact]
        // Kiểm tra GetFullOrderDetail trả về NotFound nếu không có đơn hàng
        public async Task GetFullOrderDetail_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetFullOrderDetailAsync(99)).ReturnsAsync((OrderDetailDto?)null);

            var result = await _controller.GetFullOrderDetail(99);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Không tìm thấy đơn hàng.", notFound.Value);
        }

        /* -----Test luồng hoạt động------ */
        [Fact]
        // Tạo nhiều ghế cùng lúc
        public async Task CreateMultipleOrderDetails_ReturnsAllIds()
        {
            var seatIds = new List<int> { 3, 5, 7 };
            var createdDetails = new List<OrderDetail>();

            foreach (var seatId in seatIds)
            {
                var dto = new CreateOrderDetailDto { OrderId = 1, SeatId = seatId };
                var created = new OrderDetail { Id = seatId + 100, OrderId = 1, SeatId = seatId };

                _mockService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

                var result = await _controller.CreateOrderDetail(dto);
                var okResult = Assert.IsType<OkObjectResult>(result);
                var value = okResult.Value?.GetType().GetProperty("Id")?.GetValue(okResult.Value);
                Assert.Equal(seatId + 100, (int)value);
            }
        }

        [Fact]
        // Kiểm tra khi người dùng mở UI xem chi tiết đặt vé (giao diện chi tiết đơn hàng)
        public async Task UserViewOrderDetails_ReturnsFullDetails_WithSeatsAndTotalPrice()
        {
            var dto = new OrderDetailDto
            {
                OrderId = 10,
                MovieId = 123,
                TotalPrice = 250000,
                Seats = new List<string> { "A1", "A2" }
            };

            _mockService.Setup(s => s.GetFullOrderDetailAsync(10)).ReturnsAsync(dto);

            var result = await _controller.GetFullOrderDetail(10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<OrderDetailDto>(okResult.Value);

            Assert.Equal(10, returned.OrderId);
            Assert.Equal(123, returned.MovieId);
            Assert.Equal(250000, returned.TotalPrice);
            Assert.Contains("A1", returned.Seats);
            Assert.Contains("A2", returned.Seats);
        }

        [Fact]
        // Chỉnh sửa ghế của đơn hàng từ UI (người dùng chọn lại chỗ)
        public async Task UserUpdateSeatInOrder_ReturnsUpdatedDetail()
        {
            var dto = new UpdateOrderDetailDto { SeatId = 12 };
            var updated = new OrderDetail { Id = 3, SeatId = 12, OrderId = 1 };

            _mockService.Setup(s => s.UpdateAsync(3, dto)).ReturnsAsync(updated);

            var result = await _controller.UpdateOrderDetail(3, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<OrderDetail>(okResult.Value);
            Assert.Equal(12, returned.SeatId);
        }

        [Fact]
        // Người dùng xóa (hủy) 1 ghế đã đặt
        public async Task UserDeleteSeatFromOrder_ReturnsSuccessMessage()
        {
            _mockService.Setup(s => s.DeleteAsync(3)).ReturnsAsync(true);

            var result = await _controller.DeleteOrderDetail(3);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Đã xóa chi tiết đơn hàng.", okResult.Value);
        }

        [Fact]
        // Admin mở trang quản lý đơn hàng và xem danh sách
        public async Task AdminViewAllOrderDetails_ReturnsList()
        {
            var list = new List<OrderDetail>
            {
                new OrderDetail { Id = 1, SeatId = 5, OrderId = 1 },
                new OrderDetail { Id = 2, SeatId = 6, OrderId = 1 }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

            var result = await _controller.GetAllOrderDetails();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<List<OrderDetail>>(okResult.Value);
            Assert.Equal(2, returned.Count);
        }

        [Fact]
        // Trường hợp người dùng đặt vé nhưng tạo OrderDetail bị lỗi từ service
        public async Task CreateOrderDetail_ServiceThrowsException_ReturnsBadRequest()
        {
            var dto = new CreateOrderDetailDto { OrderId = 1, SeatId = 5 };

            _mockService.Setup(s => s.CreateAsync(dto))
                .ThrowsAsync(new Exception("Seat already booked"));

            var result = await _controller.CreateOrderDetail(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Seat already booked", badRequest.Value?.ToString());
        }
    }
}