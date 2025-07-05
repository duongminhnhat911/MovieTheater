using BookingManagement.Models.Entities;
using BookingManagement.Repositories;

namespace BookingManagement.Service
{
    // Services/TransactionService.cs
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;

        public TransactionService(ITransactionRepository repo)
        {
            _repo = repo;
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
            transaction.PaymentId = updated.PaymentId;
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
    }
}
