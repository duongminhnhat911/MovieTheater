using BookingManagement.Repositories;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VnpayController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IVnpayPaymentService _vnpayService;
        private readonly ITransactionService _transactionService;

        public VnpayController(
            IOrderRepository orderRepo,
            IVnpayPaymentService vnpayService,
            ITransactionService transactionService)
        {
            _orderRepo = orderRepo;
            _vnpayService = vnpayService;
            _transactionService = transactionService;
        }

        // 1️⃣ Tạo link thanh toán
        [HttpGet("create-url")]
        public async Task<IActionResult> CreateUrl(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return NotFound("Không tìm thấy đơn hàng");

            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            string url = _vnpayService.CreatePaymentUrl(order.TotalPrice, orderId.ToString(), ip);

            return Ok(new { paymentUrl = url });
        }

        // 2️⃣ Xử lý callback (user redirect về)
        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            var result = _vnpayService.ProcessCallback(Request.Query);

            if (result.IsSuccess)
            {
                // ✅ Ghi nhận giao dịch nếu chưa ghi
                await _transactionService.SaveTransactionAsync(result);

                return Ok(new
                {
                    status = "success",
                    message = "Thanh toán thành công",
                    transactionId = result.VnpayTransactionId,
                    orderId = result.PaymentId
                });
            }

            return BadRequest(new
            {
                status = "fail",
                message = result.PaymentResponse?.Description ?? "Thanh toán thất bại"
            });
        }

        // 3️⃣ Xử lý IPN (server gọi ngầm)
        [HttpGet("ipn")]
        public async Task<IActionResult> Ipn()
        {
            var result = _vnpayService.ProcessCallback(Request.Query);
            bool saved = await _transactionService.SaveTransactionAsync(result);

            return saved ? Ok("IPN OK") : BadRequest("IPN Failed");
        }
    }
}
