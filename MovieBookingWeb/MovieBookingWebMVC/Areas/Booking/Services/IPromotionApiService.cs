using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public interface IPromotionApiService
    {
        Task<List<PromotionListViewModel>> GetAllPromotionsAsync();
        Task<bool> UpdatePromotionAsync(int id, PromotionListViewModel model);
        Task<PromotionListViewModel?> GetPromotionByIdAsync(int id);
        // IPromotionApiService.cs
        Task<bool> CreatePromotionAsync(CreatePromotionViewModel model);
    }
}
