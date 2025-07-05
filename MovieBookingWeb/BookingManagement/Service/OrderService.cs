using BookingManagement.Models.Entities;
using BookingManagement.Repositories;

namespace BookingManagement.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;

        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Order>> GetAllOrdersAsync() => await _repo.GetAllAsync();

        public async Task<Order?> GetOrderByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.Id = 0; // Đảm bảo EF sẽ auto-generate
            order.BookingDate = DateOnly.FromDateTime(DateTime.Now); // ✅ Server tự gán ngày
            await _repo.AddAsync(order);
            await _repo.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> UpdateOrderAsync(int id, Order dto)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null) return null;

            order.UserId = dto.UserId;
            order.TotalPrice = dto.TotalPrice;
            order.Status = dto.Status;
            order.BookingDate = dto.BookingDate;

            await _repo.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DisableOrderAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null) return false;

            order.Status = false;
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
