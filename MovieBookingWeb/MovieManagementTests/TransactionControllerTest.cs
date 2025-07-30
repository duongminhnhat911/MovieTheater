using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using BookingManagement.Controllers;
using BookingManagement.Models.Entities;
using BookingManagement.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingManagement.Tests.Controllers
{
    public class TransactionControllerTest
    {
        private readonly Mock<ITransactionService> _mockService;
        private readonly TransactionController _controller;

        public TransactionControllerTest()
        {
            _mockService = new Mock<ITransactionService>();
            _controller = new TransactionController(_mockService.Object);
        }

        [Fact]
        // Test CreateTransaction trả về OkObjectResult khi tạo thành công.
        public async Task CreateTransaction_ReturnsOkResult()
        {
            // Arrange
            var transaction = new Transaction { Id = 1, Price = 100000 };
            _mockService.Setup(s => s.CreateTransactionAsync(transaction))
                        .ReturnsAsync(transaction);

            // Act
            var result = await _controller.CreateTransaction(transaction);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(transaction, okResult.Value);
        }

        [Fact]
        // Test GetAllTransactions trả về danh sách transaction.
        public async Task GetAllTransactions_ReturnsAllItems()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, Price = 100000 },
                new Transaction { Id = 2, Price = 200000 }
            };
            _mockService.Setup(s => s.GetAllTransactionsAsync())
                        .ReturnsAsync(transactions);

            // Act
            var result = await _controller.GetAllTransactions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Transaction>>(okResult.Value);
            Assert.Equal(2, ((List<Transaction>)returnValue).Count);
        }

        [Fact]
        // Test GetTransactionById trả về transaction nếu tìm thấy.
        public async Task GetTransactionById_ReturnsTransaction_WhenFound()
        {
            // Arrange
            var transaction = new Transaction { Id = 1, Price = 100000 };
            _mockService.Setup(s => s.GetTransactionByIdAsync(1))
                        .ReturnsAsync(transaction);

            // Act
            var result = await _controller.GetTransactionById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(transaction, okResult.Value);
        }

        [Fact]
        // Test GetTransactionById trả về NotFound nếu không tìm thấy.
        public async Task GetTransactionById_ReturnsNotFound_WhenNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetTransactionByIdAsync(1))
                        .ReturnsAsync((Transaction)null);

            // Act
            var result = await _controller.GetTransactionById(1);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Không tìm thấy giao dịch.", notFound.Value);
        }

        [Fact]
        // Test UpdateTransaction trả về transaction đã cập nhật nếu thành công.
        public async Task UpdateTransaction_ReturnsUpdatedTransaction_WhenFound()
        {
            // Arrange
            var updated = new Transaction { Id = 1, Price = 150000 };
            _mockService.Setup(s => s.UpdateTransactionAsync(1, updated))
                        .ReturnsAsync(updated);

            // Act
            var result = await _controller.UpdateTransaction(1, updated);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updated, okResult.Value);
        }

        [Fact]
        // Test UpdateTransaction trả về NotFound nếu transaction không tồn tại.
        public async Task UpdateTransaction_ReturnsNotFound_WhenNotFound()
        {
            // Arrange
            var updated = new Transaction { Id = 1, Price = 150000 };
            _mockService.Setup(s => s.UpdateTransactionAsync(1, updated))
                        .ReturnsAsync((Transaction)null);

            // Act
            var result = await _controller.UpdateTransaction(1, updated);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Test DeleteTransaction trả về OkObjectResult khi xóa thành công.
        public async Task DeleteTransaction_ReturnsOk_WhenDeleted()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteTransactionAsync(1))
                        .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTransaction(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Transaction đã được xóa.", okResult.Value);
        }

        [Fact]
        // Test DeleteTransaction trả về NotFound nếu transaction không tồn tại.
        public async Task DeleteTransaction_ReturnsNotFound_WhenNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteTransactionAsync(1))
                        .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTransaction(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
