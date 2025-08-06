using BookingManagement.Controllers;
using BookingManagement.Models.Entities;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookingManagementTests
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
            var transaction = new Transaction { Id = 1, Price = 100000 };
            _mockService.Setup(s => s.CreateTransactionAsync(transaction))
                        .ReturnsAsync(transaction);

            var result = await _controller.CreateTransaction(transaction);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(transaction, okResult.Value);
        }

        [Fact]
        // Test GetAllTransactions trả về danh sách transaction.
        public async Task GetAllTransactions_ReturnsAllItems()
        {
            var transactions = new List<Transaction>
        {
            new Transaction { Id = 1, Price = 100000 },
            new Transaction { Id = 2, Price = 200000 }
        };
            _mockService.Setup(s => s.GetAllTransactionsAsync())
                        .ReturnsAsync(transactions);

            var result = await _controller.GetAllTransactions();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Transaction>>(okResult.Value);
            Assert.Equal(2, ((List<Transaction>)returnValue).Count);
        }

        [Fact]
        // Test GetTransactionById trả về transaction nếu tìm thấy.
        public async Task GetTransactionById_ReturnsTransaction_WhenFound()
        {
            var transaction = new Transaction { Id = 1, Price = 100000 };
            _mockService.Setup(s => s.GetTransactionByIdAsync(1))
                        .ReturnsAsync(transaction);

            var result = await _controller.GetTransactionById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(transaction, okResult.Value);
        }

        [Fact]
        // Test GetTransactionById trả về NotFound nếu không tìm thấy.
        public async Task GetTransactionById_ReturnsNotFound_WhenNotFound()
        {
            _mockService.Setup(s => s.GetTransactionByIdAsync(1))
                        .ReturnsAsync((Transaction)null);

            var result = await _controller.GetTransactionById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Không tìm thấy giao dịch.", notFound.Value);
        }

        [Fact]
        // Test UpdateTransaction trả về transaction đã cập nhật nếu thành công.
        public async Task UpdateTransaction_ReturnsUpdatedTransaction_WhenFound()
        {
            var updated = new Transaction { Id = 1, Price = 150000 };
            _mockService.Setup(s => s.UpdateTransactionAsync(1, updated))
                        .ReturnsAsync(updated);

            var result = await _controller.UpdateTransaction(1, updated);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updated, okResult.Value);
        }

        [Fact]
        // Test UpdateTransaction trả về NotFound nếu transaction không tồn tại.
        public async Task UpdateTransaction_ReturnsNotFound_WhenNotFound()
        {
            var updated = new Transaction { Id = 1, Price = 150000 };
            _mockService.Setup(s => s.UpdateTransactionAsync(1, updated))
                        .ReturnsAsync((Transaction)null);

            var result = await _controller.UpdateTransaction(1, updated);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Test DeleteTransaction trả về OkObjectResult khi xóa thành công.
        public async Task DeleteTransaction_ReturnsOk_WhenDeleted()
        {
            _mockService.Setup(s => s.DeleteTransactionAsync(1))
                        .ReturnsAsync(true);

            var result = await _controller.DeleteTransaction(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Transaction đã được xóa.", okResult.Value);
        }

        [Fact]
        // Test DeleteTransaction trả về NotFound nếu transaction không tồn tại.
        public async Task DeleteTransaction_ReturnsNotFound_WhenNotFound()
        {
            _mockService.Setup(s => s.DeleteTransactionAsync(1))
                        .ReturnsAsync(false);

            var result = await _controller.DeleteTransaction(1);

            Assert.IsType<NotFoundResult>(result);
        }

        /* -----Test luồng hoạt động------ */
        [Fact]
        // Tạo → Lấy theo ID → Cập nhật → Xóa
        public async Task CreateThenGetByIdThenUpdateThenDelete_WorksCorrectly()
        {
            var initialTransaction = new Transaction { Id = 1, Price = 100000 };
            var updatedTransaction = new Transaction { Id = 1, Price = 150000 };

            // Step 1: Create
            _mockService.Setup(s => s.CreateTransactionAsync(initialTransaction))
                        .ReturnsAsync(initialTransaction);
            var createResult = await _controller.CreateTransaction(initialTransaction);
            var created = Assert.IsType<OkObjectResult>(createResult);
            Assert.Equal(initialTransaction, created.Value);

            // Step 2: GetById
            _mockService.Setup(s => s.GetTransactionByIdAsync(1))
                        .ReturnsAsync(initialTransaction);
            var getResult = await _controller.GetTransactionById(1);
            var found = Assert.IsType<OkObjectResult>(getResult);
            Assert.Equal(initialTransaction, found.Value);

            // Step 3: Update
            _mockService.Setup(s => s.UpdateTransactionAsync(1, updatedTransaction))
                        .ReturnsAsync(updatedTransaction);
            var updateResult = await _controller.UpdateTransaction(1, updatedTransaction);
            var updated = Assert.IsType<OkObjectResult>(updateResult);
            Assert.Equal(updatedTransaction.Price, ((Transaction)updated.Value).Price);

            // Step 4: Delete
            _mockService.Setup(s => s.DeleteTransactionAsync(1))
                        .ReturnsAsync(true);
            var deleteResult = await _controller.DeleteTransaction(1);
            var deleted = Assert.IsType<OkObjectResult>(deleteResult);
            Assert.Equal("Transaction đã được xóa.", deleted.Value);
        }

        [Fact]
        // Tạo transaction không hợp lệ (null model)
        public async Task CreateTransactionWithNullModel_ReturnsBadRequest()
        {
            var result = await _controller.CreateTransaction(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thông tin giao dịch không hợp lệ.", badRequest.Value);
        }

        [Fact]
        // Cập nhật transaction không khớp ID
        public async Task UpdateTransactionWithMismatchedId_ReturnsBadRequest()
        {
            var updated = new Transaction { Id = 2, Price = 150000 };
            var result = await _controller.UpdateTransaction(1, updated);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID giao dịch không khớp.", badRequest.Value);
        }

        [Fact]
        // Xóa transaction → Không tìm thấy sau đó
        public async Task DeleteThenGetById_ReturnsNotFound()
        {
            _mockService.Setup(s => s.DeleteTransactionAsync(1)).ReturnsAsync(true);
            var deleteResult = await _controller.DeleteTransaction(1);
            var deleted = Assert.IsType<OkObjectResult>(deleteResult);

            _mockService.Setup(s => s.GetTransactionByIdAsync(1)).ReturnsAsync((Transaction)null);
            var getResult = await _controller.GetTransactionById(1);
            var notFound = Assert.IsType<NotFoundObjectResult>(getResult);
            Assert.Equal("Không tìm thấy giao dịch.", notFound.Value);
        }
    }
}
