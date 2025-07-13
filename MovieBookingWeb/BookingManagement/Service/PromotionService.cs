using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Repositories;

namespace BookingManagement.Service
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _repo;

        public PromotionService(IPromotionRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PromotionDto>> GetAllAsync()
        {
            var promos = await _repo.GetAllAsync();
            return promos.Select(p => ToDto(p)).ToList();
        }

        public async Task<PromotionDto?> GetByIdAsync(int id)
        {
            var promo = await _repo.GetByIdAsync(id);
            return promo == null ? null : ToDto(promo);
        }

        public async Task<PromotionDto> CreateAsync(CreatePromotionDto dto)
        {

            var existing = await _repo.GetByCodeAsync(dto.PromotionCode.Trim());
            if (existing != null)
                throw new InvalidOperationException("Promotion code already exists.");

            var promo = new Promotion
            {
                PromotionCode = dto.PromotionCode,
                Description = dto.Description,
                DiscountAmount = dto.DiscountAmount,
                DiscountPercent = dto.DiscountPercent,
                Quantity = dto.Quantity,
                StartDate = EnsureUtc(dto.StartDate),
                EndDate = EnsureUtc(dto.EndDate),
                IsActive = true
            };

            await _repo.AddAsync(promo);
            return ToDto(promo);
        }

        public async Task<PromotionDto?> UpdateAsync(int id, UpdatePromotionDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            existing.PromotionCode = dto.PromotionCode;
            existing.Description = dto.Description;
            existing.DiscountAmount = dto.DiscountAmount;
            existing.DiscountPercent = dto.DiscountPercent;
            existing.Quantity = dto.Quantity;
            existing.StartDate = EnsureUtc(dto.StartDate);
            existing.EndDate = EnsureUtc(dto.EndDate);
            existing.IsActive = dto.IsActive;

            await _repo.UpdateAsync(existing);
            return ToDto(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var promo = await _repo.GetByIdAsync(id);
            if (promo == null) return false;

            await _repo.RemoveAsync(promo);
            return true;
        }

        private static PromotionDto ToDto(Promotion promo) => new()
        {
            Id = promo.Id,
            PromotionCode = promo.PromotionCode,
            Description = promo.Description,
            DiscountAmount = promo.DiscountAmount,
            DiscountPercent = promo.DiscountPercent,
            Quantity = promo.Quantity,
            StartDate = promo.StartDate,
            EndDate = promo.EndDate,
            IsActive = promo.IsActive
        };

        private static DateTime EnsureUtc(DateTime dt) =>
            dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }
}
