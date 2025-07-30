using BookingManagement.Controllers;
using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text.Json;

public class OrderControllerTest
{
    private readonly Mock<IOrderService> _mockService;
    private readonly OrderController _controller;

    public OrderControllerTest()
    {
        _mockService = new Mock<IOrderService>();
        _controller = new OrderController(_mockService.Object);
    }

    private Order CreateTestOrder(int id = 1)
    {
        return new Order
        {
            Id = id,
            UserId = 100,
            BookingDate = DateOnly.FromDateTime(DateTime.Today),
            TotalPrice = 500,
            Status = true,
            PromotionId = null,
            Promotion = null
        };
    }

    [Fact]
    // Kiểm tra CreateOrder trả về OkObjectResult và thông báo đúng
    public async Task CreateOrder_ReturnsOkResult()
    {
        var order = new Order
        {
            Id = 1,
            UserId = 1,
            BookingDate = DateOnly.FromDateTime(DateTime.Today),
            TotalPrice = 100000,
            Status = true
        };

        _mockService.Setup(s => s.CreateOrderAsync(order)).ReturnsAsync(order);

        var result = await _controller.CreateOrder(order);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var json = JsonSerializer.Serialize(okResult.Value);
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        Assert.Equal("Đơn hàng đã được tạo", dict["Message"]);
    }

    [Fact]
    // Kiểm tra lấy toàn bộ đơn hàng
    public async Task GetAllOrders_ReturnsOrders()
    {
        var orders = new List<Order> { CreateTestOrder(1), CreateTestOrder(2) };
        _mockService.Setup(s => s.GetAllOrdersAsync()).ReturnsAsync(orders);

        var result = await _controller.GetAllOrders();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnOrders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
        Assert.Equal(2, returnOrders.Count());
    }

    [Fact]
    // Kiểm tra lấy đơn hàng theo ID hợp lệ
    public async Task GetOrderById_ReturnsOrder_IfExists()
    {
        var order = CreateTestOrder(1);
        _mockService.Setup(s => s.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var result = await _controller.GetOrderById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(order, okResult.Value);
    }

    [Fact]
    // Kiểm tra lấy đơn hàng không tồn tại
    public async Task GetOrderById_ReturnsNotFound_IfNotExists()
    {
        _mockService.Setup(s => s.GetOrderByIdAsync(999)).ReturnsAsync((Order)null!);

        var result = await _controller.GetOrderById(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Không tìm thấy đơn hàng.", notFound.Value);
    }

    [Fact]
    // Kiểm tra cập nhật đơn hàng thành công
    public async Task UpdateOrder_ReturnsUpdatedOrder_IfExists()
    {
        var order = CreateTestOrder();
        _mockService.Setup(s => s.UpdateOrderAsync(1, order)).ReturnsAsync(order);

        var result = await _controller.UpdateOrder(1, order);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(order, okResult.Value);
    }

    [Fact]
    // Kiểm tra cập nhật đơn hàng không tồn tại
    public async Task UpdateOrder_ReturnsNotFound_IfNotExists()
    {
        var order = CreateTestOrder();
        _mockService.Setup(s => s.UpdateOrderAsync(1, order)).ReturnsAsync((Order)null!);

        var result = await _controller.UpdateOrder(1, order);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Kiểm tra khóa đơn hàng thành công
    public async Task DisableOrder_ReturnsOk_IfSuccess()
    {
        _mockService.Setup(s => s.DisableOrderAsync(1)).ReturnsAsync(true);

        var result = await _controller.DisableOrder(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Đơn hàng đã bị khóa.", okResult.Value);
    }

    [Fact]
    // Kiểm tra khóa đơn hàng thất bại
    public async Task DisableOrder_ReturnsNotFound_IfFailed()
    {
        _mockService.Setup(s => s.DisableOrderAsync(999)).ReturnsAsync(false);

        var result = await _controller.DisableOrder(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Kiểm tra tạo thanh toán thành công
    public async Task CreatePayment_ReturnsResult_IfValid()
    {
        var dto = new CreatePaymentRequestDto { ShowtimeId = 1 };

        var response = new OrderConfirmationDto
        {
            OrderId = 101,
            UserId = 12,
            ShowtimeId = 1,
            MovieId = 55,
            BookingDate = DateOnly.FromDateTime(DateTime.Today),
            TotalPrice = 200000,
            ShowtimeDate = "2025-08-01",
            ShowtimeTime = "18:30",
            Seats = new List<string> { "A1", "A2" },
            Status = true
        };

        _mockService
            .Setup(s => s.CreatePaymentAsync(dto))
            .ReturnsAsync(response); // ✅ Trả về đúng kiểu

        var result = await _controller.CreatePayment(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<OrderConfirmationDto>(okResult.Value);
        Assert.Equal(response.OrderId, returnValue.OrderId);
        Assert.Equal(response.UserId, returnValue.UserId);
        Assert.Equal(response.ShowtimeId, returnValue.ShowtimeId);
        Assert.Equal(response.MovieId, returnValue.MovieId);
        Assert.Equal(response.BookingDate, returnValue.BookingDate);
        Assert.Equal(response.TotalPrice, returnValue.TotalPrice);
        Assert.Equal(response.Seats, returnValue.Seats);
        Assert.Equal(response.Status, returnValue.Status);
    }

    [Fact]
    // Kiểm tra tạo thanh toán thất bại do suất chiếu không tồn tại
    public async Task CreatePayment_ReturnsNotFound_IfShowtimeMissing()
    {
        var dto = new CreatePaymentRequestDto { ShowtimeId = 999 };

        _mockService
            .Setup(s => s.CreatePaymentAsync(dto))
            .ReturnsAsync((OrderConfirmationDto?)null);

        var result = await _controller.CreatePayment(dto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Suất chiếu không tồn tại", notFound.Value);
    }

    [Fact]
    // Kiểm tra tạo thanh toán lỗi (InvalidOperationException)
    public async Task CreatePayment_ReturnsBadRequest_OnInvalidOperationException()
    {
        var dto = new CreatePaymentRequestDto { ShowtimeId = 1 };

        _mockService.Setup(s => s.CreatePaymentAsync(dto))
                    .ThrowsAsync(new InvalidOperationException("Không hợp lệ"));

        var result = await _controller.CreatePayment(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);

        var json = JsonSerializer.Serialize(badRequest.Value);
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        Assert.Equal("Không hợp lệ", dict!["error"]);
    }

    [Fact]
    public async Task GetAllOrders_ReturnsAllOrders()
    {
        // Arrange
        var orders = new List<Order> { CreateTestOrder(1), CreateTestOrder(2) };
        _mockService.Setup(s => s.GetAllOrdersAsync()).ReturnsAsync(orders);

        // Act
        var result = await _controller.GetAllOrders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(orders, okResult.Value);
    }
}
