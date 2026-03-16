namespace PowerFuel.Application.DTOs.Equipment;

public record EquipmentDto(
    int Id,
    string Name,
    string? ShortDescription,
    string? LongDescription,
    string? AdditionalInfo,
    decimal Price,
    decimal? OriginalPrice,
    bool IsOnSale,
    string? ImageUrl,
    int StockQuantity,
    int CategoryId,
    string? CategoryName,
    decimal? AverageRating,
    int ReviewCount,
    int? FeaturedCoachId,
    string? FeaturedCoachName
);
