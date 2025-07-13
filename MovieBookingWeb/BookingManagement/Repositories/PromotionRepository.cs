using BookingManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly BookingDbContext _context;

        public PromotionRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<List<Promotion>> GetAllAsync() =>
            await _context.Promotions.ToListAsync();

        public async Task<Promotion?> GetByIdAsync(int id) =>
            await _context.Promotions.FindAsync(id);

        public async Task<Promotion?> GetByCodeAsync(string code) =>
            await _context.Promotions.FirstOrDefaultAsync(p => p.PromotionCode == code);

        public async Task AddAsync(Promotion promotion)
        {
            await _context.Promotions.AddAsync(promotion);
            await _context.SaveChangesAsync(); // ✅ Save ngay tại đây
        }

        public async Task UpdateAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync(); // ✅ Save ngay tại đây
        }

        public async Task RemoveAsync(Promotion promotion)
        {
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync(); // ✅ Save ngay tại đây
        }
        public async Task<Promotion?> GetPromotionByCodeAsync(string code)
        {
            return await _context.Promotions.FirstOrDefaultAsync(p => p.PromotionCode == code);
        }
    }
}
