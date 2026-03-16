namespace PowerFuel.Application.DTOs.Orders;

public record OrderDto(
    int Id,
    string Status,
    decimal SubTotal,
    decimal ShippingCost,
    decimal TotalAmount,
    string? ShippingAddress,
    DateTime OrderDate,
    IReadOnlyList<OrderItemDto> Items
);
