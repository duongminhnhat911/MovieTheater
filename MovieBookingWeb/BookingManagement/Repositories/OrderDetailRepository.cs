using BookingManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly BookingDbContext _db;

        public OrderDetailRepository(BookingDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(OrderDetail detail) =>
            await _db.OrderDetails.AddAsync(detail);

        public async Task<List<OrderDetail>> GetAllWithIncludesAsync() =>
            await _db.OrderDetails
                .Include(od => od.Seat)
                .Include(od => od.Order)
                .Include(od => od.Showtime)
                .ToListAsync();

        public async Task<OrderDetail?> GetByIdWithIncludesAsync(int id) =>
            await _db.OrderDetails
                .Include(od => od.Seat)
                .Include(od => od.Order)
                .Include(od => od.Showtime)
                .FirstOrDefaultAsync(od => od.Id == id);

        public async Task<OrderDetail?> GetByIdAsync(int id) =>
            await _db.OrderDetails.FindAsync(id);

        public void Remove(OrderDetail detail) =>
            _db.OrderDetails.Remove(detail);

        public async Task SaveChangesAsync() =>
            await _db.SaveChangesAsync();
    }
}