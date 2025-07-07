using BookingManagement.Models.Entities;

namespace BookingManagement.Repositories
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<List<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(int id, bool includeOrder = false);
        Task<Transaction?> GetByIdNoTrackingAsync(int id);
        void Remove(Transaction transaction);
        Task SaveChangesAsync();
        Task<bool> SaveTransactionAsync(IQueryCollection query);
        Task<Transaction?> GetByOrderIdAsync(int orderId);
    }
}
