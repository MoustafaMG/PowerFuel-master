using PowerFuel.Application.DTOs.Products;

namespace PowerFuel.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductListDto>> GetBestSellersAsync(int count = 8, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductListDto>> ListAsync(string? categorySlug = null, string? search = null, CancellationToken cancellationToken = default);
}
