using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Repositories;

namespace BookingManagement.Service
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _repo;

        public OrderDetailService(IOrderDetailRepository repo)
        {
            _repo = repo;
        }

        public async Task<OrderDetail> CreateAsync(CreateOrderDetailDto dto)
        {
            var detail = new OrderDetail
            {
                OrderId = dto.OrderId,
                SeatId = dto.SeatId,
                ShowtimeId = dto.ShowtimeId,
                Price = dto.Price
            };

            await _repo.AddAsync(detail);
            await _repo.SaveChangesAsync();
            return detail;
        }

        public async Task<List<OrderDetail>> GetAllAsync() =>
            await _repo.GetAllWithIncludesAsync();

        public async Task<OrderDetail?> GetByIdAsync(int id) =>
            await _repo.GetByIdWithIncludesAsync(id);

        public async Task<OrderDetail?> UpdateAsync(int id, UpdateOrderDetailDto dto)
        {
            var detail = await _repo.GetByIdAsync(id);
            if (detail == null) return null;

            if (dto.SeatId.HasValue) detail.SeatId = dto.SeatId.Value;
            if (dto.ShowtimeId.HasValue) detail.ShowtimeId = dto.ShowtimeId.Value;
            if (dto.Price.HasValue) detail.Price = dto.Price.Value;

            await _repo.SaveChangesAsync();
            return detail;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var detail = await _repo.GetByIdAsync(id);
            if (detail == null) return false;

            _repo.Remove(detail);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
