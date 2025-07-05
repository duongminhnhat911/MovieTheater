using BookingManagement.Models.Entities;

namespace BookingManagement.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task SaveChangesAsync();
    }
}
