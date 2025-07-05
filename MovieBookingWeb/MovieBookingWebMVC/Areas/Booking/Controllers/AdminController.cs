using Microsoft.AspNetCore.Mvc;

namespace MovieBookingWebMVC.Areas.Booking.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
