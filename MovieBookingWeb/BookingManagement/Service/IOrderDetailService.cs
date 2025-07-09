using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;

namespace BookingManagement.Service
{
    public interface IOrderDetailService
    {
        Task<OrderDetail> CreateAsync(CreateOrderDetailDto dto);
        Task<List<OrderDetail>> GetAllAsync();
        Task<OrderDetail?> GetByIdAsync(int id);
        Task<OrderDetail?> UpdateAsync(int id, UpdateOrderDetailDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
