using BookingManagement.Models.Entities;
using BookingManagement.Repositories;
using VNPAY.NET.Models;

namespace BookingManagement.Service
{
    // Services/TransactionService.cs
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;
        private readonly IOrderRepository _OrderRepo;

        public TransactionService(ITransactionRepository repo, IOrderRepository OrderRepo)
        {
            _repo = repo;
            _OrderRepo = OrderRepo;
        }

        public async Task<object> CreateTransactionAsync(Transaction dto)
        {
            await _repo.AddAsync(dto);
            await _repo.SaveChangesAsync();
            return new { Message = "Transaction created successfully."};
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync() =>
            await _repo.GetAllAsync();

        public async Task<Transaction?> GetTransactionByIdAsync(int id) =>
            await _repo.GetByIdAsync(id, includeOrder: true);

        public async Task<object?> UpdateTransactionAsync(int id, Transaction updated)
        {
            var transaction = await _repo.GetByIdAsync(id);
            if (transaction == null) return null;

            transaction.OrderId = updated.OrderId;
            
            transaction.TransactionDate = updated.TransactionDate;
            transaction.Price = updated.Price;
            transaction.Status = updated.Status;

            await _repo.SaveChangesAsync();

            return new { Message = "Cập nhật thành công.", transaction };
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _repo.GetByIdAsync(id);
            if (transaction == null) return false;

            _repo.Remove(transaction);
            await _repo.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SaveTransactionAsync(PaymentResult result)
        {
            if (!result.IsSuccess) return false;

            // Check xem transaction đã tồn tại chưa
            var existing = await _repo.GetByOrderIdAsync((int)result.PaymentId);
            if (existing != null) return true;

            // Lấy order
            var order = await _OrderRepo.GetByIdAsync((int)result.PaymentId);
            if (order == null) return false;

            var transaction = new Transaction
            {
                OrderId = order.Id,
                TransactionDate = DateOnly.FromDateTime(DateTime.Now),
                Price = order.TotalPrice,
                Status = true,
                
            };

            await _repo.AddAsync(transaction);

            // Cập nhật trạng thái đơn hàng
            order.Status = true;
            await _repo.SaveChangesAsync();

            return true;
        }
    }
}
