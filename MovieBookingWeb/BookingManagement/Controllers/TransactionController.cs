using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingManagement.Models.Entities;

namespace BookingManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly BookingDbContext _db;

        public TransactionController(BookingDbContext db)
        {
            _db = db;
        }

        // 1. Tạo mới transaction
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] Transaction dto)
        {
            _db.Transactions.Add(dto);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "Transaction created successfully.",
                dto.Id
            });
        }

        // 2. Lấy toàn bộ transaction
        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _db.Transactions
                .Include(t => t.Order)
                .ToListAsync();

            return Ok(transactions);
        }

        // 3. Lấy transaction theo Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var transaction = await _db.Transactions
                .Include(t => t.Order)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                return NotFound("Không tìm thấy giao dịch.");

            return Ok(transaction);
        }

        // 4. Cập nhật transaction
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] Transaction updated)
        {
            var transaction = await _db.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            transaction.OrderId = updated.OrderId;
            transaction.PaymentId = updated.PaymentId;
            transaction.TransactionDate = updated.TransactionDate;
            transaction.Price = updated.Price;
            transaction.Status = updated.Status;

            await _db.SaveChangesAsync();
            return Ok(new { Message = "Cập nhật thành công.", transaction });
        }

        // 5. Xóa transaction (hard delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _db.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            _db.Transactions.Remove(transaction);
            await _db.SaveChangesAsync();

            return Ok("Transaction đã được xóa.");
        }
    }
}
