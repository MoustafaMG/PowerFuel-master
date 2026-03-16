namespace PowerFuel.Application.DTOs.Cart;

public record CartDto(int Id, IReadOnlyList<CartItemDto> Items, decimal SubTotal, int TotalItems);
