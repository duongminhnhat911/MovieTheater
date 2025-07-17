using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;

namespace BookingManagement.Service
{
    public interface IPromotionService
    {
        Task<List<PromotionDto>> GetAllAsync();
        Task<PromotionDto?> GetByIdAsync(int id);
        Task<PromotionDto> CreateAsync(CreatePromotionDto dto);
        Task<PromotionDto?> UpdateAsync(int id, UpdatePromotionDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
