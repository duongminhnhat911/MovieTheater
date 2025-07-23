using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;
        private readonly IOrderService _orderService;


        public OrderController(IOrderService service, IOrderService orderService)
        {
            _service = service;
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var result = await _service.CreateOrderAsync(order);
            return Ok(new { Message = "Đơn hàng đã được tạo" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _service.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _service.GetOrderByIdAsync(id);
            if (order == null) return NotFound("Không tìm thấy đơn hàng.");
            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order dto)
        {
            var result = await _service.UpdateOrderAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPatch("disable/{id}")]
        public async Task<IActionResult> DisableOrder(int id)
        {
            var success = await _service.DisableOrderAsync(id);
            if (!success) return NotFound();
            return Ok("Đơn hàng đã bị khóa.");
        }

        [HttpPost("payment")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto request)
        {
            try
            {
                var result = await _service.CreatePaymentAsync(request);
                if (result == null)
                    return NotFound("Suất chiếu không tồn tại");

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        //
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _service.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }
    }
}