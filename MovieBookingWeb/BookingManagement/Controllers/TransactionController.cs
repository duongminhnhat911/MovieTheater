using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingManagement.Models.Entities;
using BookingManagement.Service;

namespace BookingManagement.Controllers
{
    // Controllers/TransactionController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] Transaction dto)
        {
            var result = await _service.CreateTransactionAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _service.GetAllTransactionsAsync();
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var transaction = await _service.GetTransactionByIdAsync(id);
            if (transaction == null)
                return NotFound("Không tìm thấy giao dịch.");
            return Ok(transaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] Transaction updated)
        {
            var result = await _service.UpdateTransactionAsync(id, updated);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var success = await _service.DeleteTransactionAsync(id);
            if (!success) return NotFound();
            return Ok("Transaction đã được xóa.");
        }
    }
}
