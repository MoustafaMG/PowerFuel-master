using PowerFuel.Application.DTOs.Categories;

namespace PowerFuel.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetProductCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryDto>> GetEquipmentCategoriesAsync(CancellationToken cancellationToken = default);
}
