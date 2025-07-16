using BookingManagement.Models.DTOs;
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
        public async Task<List<OrderHistoryDto>> GetOrderHistoryByUserIdAsync(int userId)
        {
            // 1. Truy vấn dữ liệu đã include
            var orders = await _db.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Seat)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Showtime)
                        .ThenInclude(st => st.Room)
                .OrderByDescending(o => o.BookingDate)
                .ToListAsync(); // 👈 THỰC THI SQL ở đây

            // 2. Dùng LINQ to Objects (không bị hạn chế bởi EF)
            var result = orders.Select(o =>
            {
                var firstDetail = o.OrderDetails.FirstOrDefault();

                return new OrderHistoryDto
                {
                    OrderId = o.Id,
                    ShowDate = firstDetail?.Showtime?.ShowDate ?? default,
                    FromTime = firstDetail?.Showtime?.FromTime ?? default,
                    RoomName = firstDetail?.Showtime?.Room?.RoomName ?? "Không rõ",
                    Seats = o.OrderDetails
                        .Where(od => od.Seat != null)
                        .Select(od => od.Seat.SeatRow.ToString() + od.Seat.SeatColumn.ToString())
                        .ToList(),
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                    BookingDate = o.BookingDate
                };
            }).ToList();

            return result;
        }
    }
}
