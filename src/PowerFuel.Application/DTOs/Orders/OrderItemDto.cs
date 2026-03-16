namespace PowerFuel.Application.DTOs.Orders;

public record OrderItemDto(int Id, string ItemName, string ItemType, decimal UnitPrice, int Quantity, decimal LineTotal);
