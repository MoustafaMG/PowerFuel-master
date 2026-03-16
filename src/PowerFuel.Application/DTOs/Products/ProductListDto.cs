namespace PowerFuel.Application.DTOs.Products;

public record ProductListDto(
    int Id,
    string Name,
    string? ShortDescription,
    decimal Price,
    decimal? OriginalPrice,
    bool IsOnSale,
    string? ImageUrl,
    decimal? AverageRating,
    int ReviewCount,
    string? CategoryName
);
