namespace PowerFuel.Application.DTOs.Cart;

public record AddToCartRequest(string ItemType, int ItemId, int Quantity = 1);
