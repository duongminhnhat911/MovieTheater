using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;

namespace BookingManagement.Service
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> UpdateOrderAsync(int id, Order dto);
        Task<bool> DisableOrderAsync(int id);
        Task<OrderConfirmationDto?> CreatePaymentAsync(CreatePaymentRequestDto request);
        Task<List<OrderHistoryDto>> GetOrderHistoryAsync(int userId);
    }
}
