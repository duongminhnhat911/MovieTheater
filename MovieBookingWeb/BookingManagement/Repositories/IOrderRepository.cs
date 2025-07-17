using BookingManagement.Models.Entities;
using BookingManagement.Models.DTOs;

namespace BookingManagement.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task SaveChangesAsync();
        Task CreateOrderAsync(Order order);
        Task AddOrderDetailAsync(OrderDetail detail);
        Task UpdatePromotionAsync(Promotion promo);

        Task<Promotion?> GetPromotionByCodeAsync(string code);

        Task<List<OrderHistoryDto>> GetOrderHistoryByUserIdAsync(int userId);
    }
}
