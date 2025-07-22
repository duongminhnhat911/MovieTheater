using BookingManagement.Models.Entities;

namespace BookingManagement.Repositories
{
    public interface IOrderDetailRepository
    {
        Task AddAsync(OrderDetail detail);
        Task<List<OrderDetail>> GetAllWithIncludesAsync();
        Task<OrderDetail?> GetByIdWithIncludesAsync(int id);
        Task<OrderDetail?> GetByIdAsync(int id);
        void Remove(OrderDetail detail);
        Task SaveChangesAsync();

        Task<List<OrderDetail>> GetByOrderIdWithIncludesAsync(int orderId);

    }
}
