using BookingManagement.Controllers;
using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookingManagement.Tests.Controllers
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
            // Arrange
            var dto = new CreateOrderDetailDto { OrderId = 1, SeatId = 5 };
            var createdDetail = new OrderDetail { Id = 100, OrderId = 1, SeatId = 5 };
            _mockService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(createdDetail);

            // Act
            var result = await _controller.CreateOrderDetail(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;

            // Ép kiểu lại thành Dictionary<string, object>
            var idProperty = value?.GetType().GetProperty("Id")?.GetValue(value, null);
            Assert.NotNull(idProperty);
            Assert.Equal(100, (int)idProperty);
        }

        [Fact]
        // Kiểm tra lấy danh sách OrderDetail trả về đúng kiểu và số lượng
        public async Task GetAllOrderDetails_ReturnsOk_WithList()
        {
            // Arrange
            var details = new List<OrderDetail> {
                new OrderDetail { Id = 1 }, new OrderDetail { Id = 2 }
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(details);

            // Act
            var result = await _controller.GetAllOrderDetails();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<List<OrderDetail>>(okResult.Value);
            Assert.Equal(2, returned.Count);
        }

        [Fact]
        // Kiểm tra lấy chi tiết đơn hàng theo ID hợp lệ
        public async Task GetOrderDetailById_ExistingId_ReturnsOk()
        {
            // Arrange
            var detail = new OrderDetail { Id = 5 };
            _mockService.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(detail);

            // Act
            var result = await _controller.GetOrderDetailById(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<OrderDetail>(okResult.Value);
            Assert.Equal(5, returned.Id);
        }

        [Fact]
        // Kiểm tra controller trả về NotFound khi không có OrderDetail
        public async Task GetOrderDetailById_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((OrderDetail?)null);

            // Act
            var result = await _controller.GetOrderDetailById(999);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Không tìm thấy chi tiết đơn hàng.", notFound.Value);
        }

        [Fact]
        // Kiểm tra cập nhật OrderDetail thành công
        public async Task UpdateOrderDetail_ExistingId_ReturnsOk()
        {
            // Arrange
            var dto = new UpdateOrderDetailDto { SeatId = 10 };
            var updated = new OrderDetail { Id = 1, SeatId = 10 };
            _mockService.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(updated);

            // Act
            var result = await _controller.UpdateOrderDetail(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<OrderDetail>(okResult.Value);
            Assert.Equal(10, returned.SeatId);
        }

        [Fact]
        // Kiểm tra cập nhật thất bại nếu không tìm thấy OrderDetail
        public async Task UpdateOrderDetail_NotFound_ReturnsNotFound()
        {
            // Arrange
            var dto = new UpdateOrderDetailDto { SeatId = 10 };
            _mockService.Setup(s => s.UpdateAsync(99, dto)).ReturnsAsync((OrderDetail?)null);

            // Act
            var result = await _controller.UpdateOrderDetail(99, dto);

            // Assert
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
    }
}
