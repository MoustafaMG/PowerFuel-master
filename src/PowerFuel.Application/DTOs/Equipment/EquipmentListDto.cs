namespace PowerFuel.Application.DTOs.Equipment;

public record EquipmentListDto(
    int Id,
    string Name,
    string? ShortDescription,
    decimal Price,
    decimal? OriginalPrice,
    bool IsOnSale,
    string? ImageUrl,
    string? CategoryName,
    int? FeaturedCoachId
);
