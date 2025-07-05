using BookingManagement.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly BookingDbContext _db;

        public OrderController(BookingDbContext db)
        {
            _db = db;
        }

        // [POST] Tạo Order mới
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            order.BookingDate = DateOnly.FromDateTime(DateTime.Now);
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "Đơn hàng đã được tạo", order.Id });
        }

        // [GET] Lấy tất cả Order
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _db.Orders.ToListAsync();
            return Ok(orders);
        }

        // [GET] Lấy Order theo Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound("Không tìm thấy đơn hàng.");
            return Ok(order);
        }

        // [PUT] Cập nhật Order
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order dto)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.UserId = dto.UserId;
            order.TotalPrice = dto.TotalPrice;
            order.Status = dto.Status;
            order.BookingDate = dto.BookingDate;

            await _db.SaveChangesAsync();
            return Ok(order);
        }

        // [PATCH] Khóa đơn (soft-delete)
        [HttpPatch("disable/{id}")]
        public async Task<IActionResult> DisableOrder(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = false;
            await _db.SaveChangesAsync();
            return Ok("Đơn hàng đã bị khóa.");
        }
    }
}