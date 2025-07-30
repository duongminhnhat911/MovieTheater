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
        public async Task<bool> SaveTransactionAsync(IQueryCollection query)
        {
            var orderId = int.Parse(query["vnp_TxnRef"]);
            var transactionId = long.Parse(query["vnp_TransactionNo"]);
            var amount = int.Parse(query["vnp_Amount"]) / 100;
            var status = query["vnp_TransactionStatus"] == "00";

            var exists = await _db.Transactions.AnyAsync(t => t.OrderId == orderId);
            if (exists) return false;

            var transaction = new Transaction
            {
                OrderId = orderId,
                PaymentId = (int)transactionId,
                TransactionDate = DateOnly.FromDateTime(DateTime.Now),
                Price = amount,
                Status = status
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            return true;
        }
        public async Task<Transaction?> GetByOrderIdAsync(int orderId)
        {
            return await _db.Transactions
                .FirstOrDefaultAsync(t => t.OrderId == orderId);
        }
    }
}
