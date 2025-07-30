using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using BookingManagement.Controllers;
using BookingManagement.Models.Entities;
using BookingManagement.Service;

namespace BookingManagement.Tests.Controllers
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _mockService;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _mockService = new Mock<ITransactionService>();
            _controller = new TransactionController(_mockService.Object);
        }

        [Fact]
        // Test kiểm tra khi tạo transaction thành công, trả về kết quả Ok kèm transaction đã tạo.
        public async Task CreateTransaction_ReturnsOkWithCreatedTransaction()
        {
            // Arrange
            var transaction = new Transaction { Id = 1, OrderId = 1, Price = 100, Status = true };
            _mockService.Setup(s => s.CreateTransactionAsync(transaction))
                        .ReturnsAsync(transaction);

            // Act
            var result = await _controller.CreateTransaction(transaction);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(transaction);
        }

        [Fact]
        // Test kiểm tra lấy tất cả transactions trả về danh sách với status code 200 OK.
        public async Task GetAllTransactions_ReturnsOkWithList()
        {
            // Arrange
            var list = new List<Transaction>
            {
                new Transaction { Id = 1, OrderId = 1 },
                new Transaction { Id = 2, OrderId = 2 }
            };
            _mockService.Setup(s => s.GetAllTransactionsAsync()).ReturnsAsync(list);

            // Act
            var result = await _controller.GetAllTransactions();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            var value = okResult!.Value as List<Transaction>;
            value.Should().HaveCount(2);
        }

        [Fact]
        // Test kiểm tra lấy transaction theo ID tồn tại, trả về Ok với dữ liệu đúng.
        public async Task GetTransactionById_Exists_ReturnsOk()
        {
            // Arrange
            var transaction = new Transaction { Id = 1, OrderId = 1 };
            _mockService.Setup(s => s.GetTransactionByIdAsync(1)).ReturnsAsync(transaction);

            // Act
            var result = await _controller.GetTransactionById(1);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(transaction);
        }

        [Fact]
        // Test kiểm tra lấy transaction theo ID không tồn tại, trả về NotFound.
        public async Task GetTransactionById_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetTransactionByIdAsync(99)).ReturnsAsync((Transaction?)null);

            var result = await _controller.GetTransactionById(99);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        // Test kiểm tra cập nhật transaction thành công, trả về Ok với transaction đã cập nhật.
        public async Task UpdateTransaction_Exists_ReturnsOk()
        {
            // Arrange
            var transaction = new Transaction { Id = 1, Price = 200 };
            _mockService.Setup(s => s.UpdateTransactionAsync(1, transaction)).ReturnsAsync(transaction);

            // Act
            var result = await _controller.UpdateTransaction(1, transaction);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(transaction);
        }

        [Fact]
        // Test kiểm tra cập nhật transaction không tồn tại, trả về NotFound.
        public async Task UpdateTransaction_NotFound_ReturnsNotFound()
        {
            var updated = new Transaction { Id = 99 };
            _mockService.Setup(s => s.UpdateTransactionAsync(99, updated)).ReturnsAsync((Transaction?)null);

            var result = await _controller.UpdateTransaction(99, updated);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        // Test kiểm tra xoá transaction thành công, trả về Ok.
        public async Task DeleteTransaction_Success_ReturnsOk()
        {
            _mockService.Setup(s => s.DeleteTransactionAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteTransaction(1);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        // Test kiểm tra xoá transaction không tồn tại, trả về NotFound.
        public async Task DeleteTransaction_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.DeleteTransactionAsync(999)).ReturnsAsync(false);

            var result = await _controller.DeleteTransaction(999);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
