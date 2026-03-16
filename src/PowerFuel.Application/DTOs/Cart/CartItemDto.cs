namespace PowerFuel.Application.DTOs.Cart;

public record CartItemDto(
    int Id,
    int? ProductId,
    int? EquipmentId,
    string ItemName,
    string ItemType,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal,
    string? ImageUrl
);
