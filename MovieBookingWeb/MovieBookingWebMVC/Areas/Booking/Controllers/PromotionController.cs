using Microsoft.AspNetCore.Mvc;
using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;
using MovieBookingWebMVC.Areas.Booking.Services;

namespace MovieBookingWebMVC.Areas.Booking.Controllers
{
    [Area("Booking")]
    public class PromotionController : Controller
    {
        private readonly IPromotionApiService _promotionService;

        public PromotionController(IPromotionApiService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        public async Task<IActionResult> ShowPromotionList()
        {
            var promotions = await _promotionService.GetAllPromotionsAsync();
            var activePromotions = promotions
                .Where(p => p.IsActive && p.EndDate >= DateTime.Now)
                .ToList();
            return View(activePromotions);
        }
    }
}