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
        public async Task CreateOrderAsync(Order order)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
        }

        public async Task AddOrderDetailAsync(OrderDetail detail)
        {
            _db.OrderDetails.Add(detail);
            await _db.SaveChangesAsync();
        }

        public async Task UpdatePromotionAsync(Promotion promo)
        {
            _db.Promotions.Update(promo);
            await _db.SaveChangesAsync();
        }

        public async Task<Promotion?> GetPromotionByCodeAsync(string code)
        {
            return await _db.Promotions.FirstOrDefaultAsync(p => p.PromotionCode == code);
        }
        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _db.Orders
                                 .Where(o => o.UserId == userId)
                                 .ToListAsync();
        }

    }
}
