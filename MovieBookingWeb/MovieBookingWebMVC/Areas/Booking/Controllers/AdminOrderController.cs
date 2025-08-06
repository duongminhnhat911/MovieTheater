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
        private const int PageSize = 5;

        public AdminOrderController(IOrderApiService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> ViewOrders(int page = 1)
        {
            var allOrders = await _orderService.GetAllOrdersAsync();

            var totalOrders = allOrders.Count;
            var totalPages = (int)Math.Ceiling((double)totalOrders / PageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var pagedOrders = allOrders
                .OrderByDescending(o => o.BookingDate)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedOrders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetailsPartial([FromRoute(Name = "id")] int orderId)
        {
            var details = await _orderService.GetOrderDetailsAsync(orderId);
            return PartialView("_OrderDetailsPartial", details);
        }
    }
}
