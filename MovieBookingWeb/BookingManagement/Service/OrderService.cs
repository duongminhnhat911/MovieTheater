using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Repositories;

namespace BookingManagement.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IShowtimeRepository _showtimeRepository;
        private readonly ISeatShowtimeRepository _seatShowtimeRepo;
        private readonly ISeatRepository _seatRepo;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository repo, IShowtimeRepository showtimeRepository, ISeatShowtimeRepository seatShowtimeRepo, ISeatRepository seatRepository, IOrderRepository orderRepository)

        {
            _repo = repo;
            _showtimeRepository = showtimeRepository;
            _seatShowtimeRepo = seatShowtimeRepo;
            _seatRepo = seatRepository;
            _orderRepository = orderRepository;
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
        public async Task<OrderConfirmationDto?> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            const int ticketPrice = 80000;
            var total = request.SeatIds.Count * ticketPrice;

            var showtime = await _showtimeRepository.GetShowtimeByIdAsync(request.ShowtimeId);
            if (showtime == null) return null;

            Promotion? promo = null;
            int? promoId = null;

            // Xử lý Promotion nếu có
            if (!string.IsNullOrWhiteSpace(request.PromotionCode))
            {
                promo = await _repo.GetPromotionByCodeAsync(request.PromotionCode.Trim());
                if (promo == null)
                    throw new InvalidOperationException("Mã khuyến mãi không tồn tại.");

                var now = DateTime.UtcNow;
                if (!promo.IsActive || promo.Quantity <= 0 || now < promo.StartDate || now > promo.EndDate)
                    throw new InvalidOperationException("Mã khuyến mãi không còn hiệu lực.");

                // Áp dụng giảm giá
                if (promo.DiscountAmount.HasValue)
                    total -= promo.DiscountAmount.Value;
                else if (promo.DiscountPercent.HasValue)
                    total -= (total * promo.DiscountPercent.Value) / 100;

                promoId = promo.Id;

                // Giảm số lượng mã
                promo.Quantity -= 1;
            }

            if (total < 0) total = 0;

            var order = new Order
            {
                UserId = request.UserId,
                BookingDate = DateOnly.FromDateTime(DateTime.Now),
                TotalPrice = total,
                Status = false,
                PromotionId = promoId
            };

            await _repo.CreateOrderAsync(order);

            foreach (var seatId in request.SeatIds)
            {
                await _repo.AddOrderDetailAsync(new OrderDetail
                {
                    OrderId = order.Id,
                    SeatId = seatId,
                    ShowtimeId = request.ShowtimeId,
                    Price = ticketPrice
                });

                await _seatShowtimeRepo.HoldSeatAsync(seatId, request.ShowtimeId);
            }

            var seatNames = await _seatRepo.GetSeatNamesAsync(request.SeatIds);

            return new OrderConfirmationDto
            {
                OrderId = order.Id,
                UserId = request.UserId,
                ShowtimeId = request.ShowtimeId,
                MovieId = showtime.MovieId,
                BookingDate = order.BookingDate,
                TotalPrice = total,
                ShowtimeDate = showtime.ShowDate.ToString("yyyy-MM-dd"),
                ShowtimeTime = showtime.FromTime.ToString("HH:mm"),
                Seats = seatNames,
                Status = order.Status
            };
        }

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(int userId)
        {
            return await _orderRepository.GetOrderHistoryByUserIdAsync(userId);
        }

    }
}
