using BookingManagement.Models.Entities;

namespace BookingManagement.Service
{
    public interface ITransactionService
    {
        Task<object> CreateTransactionAsync(Transaction dto);
        Task<List<Transaction>> GetAllTransactionsAsync();
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task<object?> UpdateTransactionAsync(int id, Transaction updated);
        Task<bool> DeleteTransactionAsync(int id);
    }
}
