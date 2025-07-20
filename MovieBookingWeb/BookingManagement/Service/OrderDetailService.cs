using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task<OrderDetailDto?> GetFullOrderDetailAsync(int orderId)
        {
            var details = await _repo.GetByOrderIdWithIncludesAsync(orderId);
            if (details == null || !details.Any()) return null;

            var first = details.First();

            return new OrderDetailDto
            {
                OrderId = first.OrderId,
                UserId = first.Order?.UserId ?? 0,
                BookingDate = first.Order?.BookingDate.ToString("yyyy-MM-dd") ?? "",
                TotalPrice = first.Order?.TotalPrice ?? 0,
                Status = first.Order?.Status == true ? "Đã thanh toán" : "Chưa thanh toán",
                PromotionCode = first.Order?.Promotion?.PromotionCode ?? "Không áp dụng",


                MovieId = first.Showtime?.MovieId ?? 0,


                ShowDate = first.Showtime?.ShowDate.ToString("yyyy-MM-dd") ?? "",
                FromTime = first.Showtime?.FromTime.ToString(@"hh\\:mm") ?? "",
                ToTime = first.Showtime?.ToTime.ToString(@"hh\\:mm") ?? "",
                RoomName = first.Seat?.Room?.RoomName ?? "",

                Seats = details
                    .Where(d => d.Seat != null)
                    .Select(d => $"{d.Seat!.SeatRow}{d.Seat.SeatColumn}")
                    .ToList(),


                TotalDetailPrice = details.Sum(x => x.Price)
            };
        }


    }
}
