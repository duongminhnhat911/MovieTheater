using BookingManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly BookingDbContext _db;

        public OrderRepository(BookingDbContext db)
        {
            _db = db;
        }

        public async Task<List<Order>> GetAllAsync() => await _db.Orders.ToListAsync();

        public async Task<Order?> GetByIdAsync(int id) => await _db.Orders.FindAsync(id);

        public async Task AddAsync(Order order) => await _db.Orders.AddAsync(order);

        public async Task UpdateAsync(Order order) => _db.Orders.Update(order);

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}
