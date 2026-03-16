using Microsoft.EntityFrameworkCore;
using PowerFuel.Application.DTOs.Categories;
using PowerFuel.Application.Interfaces;
using PowerFuel.Infrastructure.Data;

namespace PowerFuel.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<CategoryDto>> GetProductCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories.AsNoTracking()
            .Where(c => c.Kind == "Product")
            .Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.Description))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CategoryDto>> GetEquipmentCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories.AsNoTracking()
            .Where(c => c.Kind == "Equipment")
            .Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.Description))
            .ToListAsync(cancellationToken);
    }
}
