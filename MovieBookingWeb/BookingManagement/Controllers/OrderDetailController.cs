using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Controllers
{
    // Controllers/OrderDetailController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _service;

        public OrderDetailController(IOrderDetailService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderDetail([FromBody] CreateOrderDetailDto dto)
        {
            var detail = await _service.CreateAsync(dto);
            return Ok(new { detail.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrderDetails()
        {
            var details = await _service.GetAllAsync();
            return Ok(details);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetailById(int id)
        {
            var detail = await _service.GetByIdAsync(id);
            if (detail == null) return NotFound("Không tìm thấy chi tiết đơn hàng.");
            return Ok(detail);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderDetail(int id, [FromBody] UpdateOrderDetailDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok("Đã xóa chi tiết đơn hàng.");
        }
    }
}