using Microsoft.EntityFrameworkCore;
using PowerFuel.Application.DTOs.Equipment;
using PowerFuel.Application.Interfaces;
using PowerFuel.Infrastructure.Data;

namespace PowerFuel.Infrastructure.Services;

public class EquipmentService : IEquipmentService
{
    private readonly ApplicationDbContext _context;

    public EquipmentService(ApplicationDbContext context) => _context = context;

    public async Task<EquipmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var e = await _context.Equipments.AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.FeaturedCoach)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (e == null) return null;
        var coachName = e.FeaturedCoach != null ? $"{e.FeaturedCoach.FirstName} {e.FeaturedCoach.LastName}" : null;
        return new EquipmentDto(
            e.Id,
            e.Name,
            e.ShortDescription,
            e.LongDescription,
            e.AdditionalInfo,
            e.Price,
            e.OriginalPrice,
            e.IsOnSale,
            e.ImageUrl,
            e.StockQuantity,
            e.CategoryId,
            e.Category?.Name,
            e.AverageRating,
            e.ReviewCount,
            e.FeaturedCoachId,
            coachName
        );
    }

    public async Task<IReadOnlyList<EquipmentListDto>> ListAsync(string? categorySlug = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Equipments.AsNoTracking().Include(x => x.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(categorySlug))
            query = query.Where(e => e.Category != null && e.Category.Slug == categorySlug);
        return await query
            .Select(e => new EquipmentListDto(
                e.Id,
                e.Name,
                e.ShortDescription,
                e.Price,
                e.OriginalPrice,
                e.IsOnSale,
                e.ImageUrl,
                e.Category != null ? e.Category.Name : null,
                e.FeaturedCoachId
            ))
            .ToListAsync(cancellationToken);
    }
}
