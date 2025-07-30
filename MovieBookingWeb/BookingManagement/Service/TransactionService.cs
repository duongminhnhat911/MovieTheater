using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using BookingManagement.Repositories;
using VNPAY.NET.Models;

namespace BookingManagement.Service
{
    // Services/TransactionService.cs
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;
        private readonly IOrderRepository _OrderRepo;
        private readonly ISeatShowtimeRepository _seatShowtimeRepo;
        private readonly IOrderDetailRepository _orderDetailRepo;

        public TransactionService(ITransactionRepository repo, IOrderRepository OrderRepo, ISeatShowtimeRepository seatShowtimeRepo, IOrderDetailRepository orderDetailRepo)
        {
            _repo = repo;
            _OrderRepo = OrderRepo;
            _seatShowtimeRepo = seatShowtimeRepo;
            _orderDetailRepo = orderDetailRepo;
        }

        public async Task<object> CreateTransactionAsync(Transaction dto)
        {
            await _repo.AddAsync(dto);
            await _repo.SaveChangesAsync();
            return new { Message = "Transaction created successfully." };
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync() =>
            await _repo.GetAllAsync();

        public async Task<Transaction?> GetTransactionByIdAsync(int id) =>
            await _repo.GetByIdAsync(id, includeOrder: true);

        public async Task<object?> UpdateTransactionAsync(int id, Transaction updated)
        {
            var transaction = await _repo.GetByIdAsync(id);
            if (transaction == null) return null;

            transaction.OrderId = updated.OrderId;

            transaction.TransactionDate = updated.TransactionDate;
            transaction.Price = updated.Price;
            transaction.Status = updated.Status;

            await _repo.SaveChangesAsync();

            return new { Message = "Cập nhật thành công.", transaction };
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _repo.GetByIdAsync(id);
            if (transaction == null) return false;

            _repo.Remove(transaction);
            await _repo.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SaveTransactionAsync(PaymentResult result)
        {
            if (!result.IsSuccess) return false;

            // Tách orderId từ Description
            int orderId;
            if (!string.IsNullOrEmpty(result.Description) && result.Description.Contains("OrderId:"))
            {
                string[] parts = result.Description.Split("OrderId:");
                if (parts.Length == 2 && int.TryParse(parts[1], out var parsedId))
                {
                    orderId = parsedId;
                }
                else return false;
            }
            else return false;

            var existing = await _repo.GetByOrderIdAsync(orderId);
            if (existing != null) return true;

            var order = await _OrderRepo.GetByIdAsync(orderId);
            if (order == null) return false;

            var transaction = new Transaction
            {
                OrderId = order.Id,
                TransactionDate = DateOnly.FromDateTime(DateTime.Now),
                Price = order.TotalPrice,
                Status = true,
            };

            await _repo.AddAsync(transaction);

            var orderDetails = await _orderDetailRepo.GetAllWithIncludesAsync();
            var relatedDetails = orderDetails.Where(d => d.OrderId == order.Id);

            foreach (var detail in relatedDetails)
            {
                var seatShowtime = await _seatShowtimeRepo.GetAsync(detail.ShowtimeId, detail.SeatId);
                if (seatShowtime != null)
                {
                    seatShowtime.Status = SeatStatus.Booked;
                }
            }

            order.Status = true;
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
