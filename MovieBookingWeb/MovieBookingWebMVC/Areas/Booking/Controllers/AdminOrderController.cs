using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingWebMVC.Areas.Booking.Services;

namespace MovieBookingWebMVC.Areas.Booking.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Booking")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderApiService _orderService;

        public AdminOrderController(IOrderApiService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> ViewOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetailsPartial([FromRoute(Name = "id")] int orderId)
        {
            var details = await _orderService.GetOrderDetailsAsync(orderId);
            return PartialView("_OrderDetailsPartial", details);
        }
    }
}
