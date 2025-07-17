using BookingManagement.Models.Entities;

namespace BookingManagement.Repositories
{
    public interface IPromotionRepository
    {
        Task<List<Promotion>> GetAllAsync();
        Task<Promotion?> GetByIdAsync(int id);
        Task<Promotion?> GetByCodeAsync(string code);
        Task AddAsync(Promotion promotion);
        Task UpdateAsync(Promotion promotion);
        Task RemoveAsync(Promotion promotion);
        Task<Promotion?> GetPromotionByCodeAsync(string code);
    }
}
