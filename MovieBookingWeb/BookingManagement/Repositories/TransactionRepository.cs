using BookingManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Repositories
{
    // Repositories/TransactionRepository.cs
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BookingDbContext _db;

        public TransactionRepository(BookingDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Transaction transaction) =>
            await _db.Transactions.AddAsync(transaction);

        public async Task<List<Transaction>> GetAllAsync() =>
            await _db.Transactions.Include(t => t.Order).ToListAsync();

        public async Task<Transaction?> GetByIdAsync(int id, bool includeOrder = false)
        {
            if (includeOrder)
            {
                return await _db.Transactions
                    .Include(t => t.Order)
                    .FirstOrDefaultAsync(t => t.Id == id);
            }

            return await _db.Transactions.FindAsync(id);
        }

        public async Task<Transaction?> GetByIdNoTrackingAsync(int id) =>
            await _db.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

        public void Remove(Transaction transaction) =>
            _db.Transactions.Remove(transaction);

        public async Task SaveChangesAsync() =>
            await _db.SaveChangesAsync();
    }
}
