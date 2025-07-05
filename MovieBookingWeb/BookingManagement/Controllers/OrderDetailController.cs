using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly BookingDbContext _db;

        public OrderDetailController(BookingDbContext db)
        {
            _db = db;
        }

        // [POST] Tạo OrderDetail
        [HttpPost]
        public async Task<IActionResult> CreateOrderDetail([FromBody] CreateOrderDetailDto dto)
        {
            var detail = new OrderDetail
            {
                OrderId = dto.OrderId,
                SeatId = dto.SeatId,
                ShowtimeId = dto.ShowtimeId,
                Price = dto.Price
            };

            _db.OrderDetails.Add(detail);
            await _db.SaveChangesAsync();
            return Ok(new { detail.Id });
        }

        // [GET] Lấy tất cả OrderDetails
        [HttpGet]
        public async Task<IActionResult> GetAllOrderDetails()
        {
            var details = await _db.OrderDetails
                .Include(od => od.Seat)
                .Include(od => od.Order)
                .Include(od => od.Showtime)
                .ToListAsync();

            return Ok(details);
        }

        // [GET] Lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetailById(int id)
        {
            var detail = await _db.OrderDetails
                .Include(od => od.Seat)
                .Include(od => od.Order)
                .Include(od => od.Showtime)
                .FirstOrDefaultAsync(od => od.Id == id);

            if (detail == null) return NotFound("Không tìm thấy chi tiết đơn hàng.");
            return Ok(detail);
        }

        // [PUT] Cập nhật OrderDetail
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderDetail(int id, [FromBody] UpdateOrderDetailDto dto)
        {
            var detail = await _db.OrderDetails.FindAsync(id);
            if (detail == null) return NotFound();

            if (dto.SeatId.HasValue) detail.SeatId = dto.SeatId.Value;
            if (dto.ShowtimeId.HasValue) detail.ShowtimeId = dto.ShowtimeId.Value;
            if (dto.Price.HasValue) detail.Price = dto.Price.Value;

            await _db.SaveChangesAsync();
            return Ok(detail);
        }

        // [DELETE] Xóa chi tiết đơn hàng
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var detail = await _db.OrderDetails.FindAsync(id);
            if (detail == null) return NotFound();

            _db.OrderDetails.Remove(detail);
            await _db.SaveChangesAsync();

            return Ok("Đã xóa chi tiết đơn hàng.");
        }
    }
}