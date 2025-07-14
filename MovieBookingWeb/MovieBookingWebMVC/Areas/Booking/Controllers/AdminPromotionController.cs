using Microsoft.AspNetCore.Mvc;
using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;
using MovieBookingWebMVC.Areas.Booking.Services;

namespace MovieBookingWebMVC.Areas.Booking.Controllers
{
    [Area("Booking")]
    public class AdminPromotionController : Controller
    {
        private readonly IPromotionApiService _promotionService;

        public AdminPromotionController(IPromotionApiService promotionService)
        {
            _promotionService = promotionService;
        }

        public async Task<IActionResult> ListPromotions()
        {
            var promotions = await _promotionService.GetAllPromotionsAsync();
            return View(promotions);
        }

        // GET: Booking/AdminPromotion/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var promo = await _promotionService.GetPromotionByIdAsync(id);
            if (promo == null) return NotFound();

            return View(promo); // chuyển đến view Edit.cshtml
        }

        // POST: Booking/AdminPromotion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PromotionListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _promotionService.UpdatePromotionAsync(id, model);
            if (!result)
            {
                ModelState.AddModelError("", "Không thể cập nhật khuyến mãi.");
                return View(model);
            }

            return RedirectToAction("ListPromotions");
        }
        // GET: Booking/AdminPromotion/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Booking/AdminPromotion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePromotionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _promotionService.CreatePromotionAsync(model);
            if (!result)
            {
                ModelState.AddModelError("", "Không thể tạo khuyến mãi.");
                return View(model);
            }

            return RedirectToAction("ListPromotions");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _promotionService.DeletePromotionAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = $"Đã xóa thành công khuyến mãi ID = {id}";
            }
            else
            {
                TempData["ErrorMessage"] = $"❌ Xóa thất bại! Không tìm thấy khuyến mãi ID = {id}";
            }

            return RedirectToAction(nameof(ListPromotions));
        }
    }
}
