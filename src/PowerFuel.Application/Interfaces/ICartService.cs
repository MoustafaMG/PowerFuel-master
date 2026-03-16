using PowerFuel.Application.DTOs.Cart;

namespace PowerFuel.Application.Interfaces;

public interface ICartService
{
    Task<CartDto?> GetCartAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<CartDto?> AddItemAsync(Guid userId, AddToCartRequest request, CancellationToken cancellationToken = default);
    Task<CartDto?> UpdateQuantityAsync(Guid userId, int cartItemId, int quantity, CancellationToken cancellationToken = default);
    Task<bool> RemoveItemAsync(Guid userId, int cartItemId, CancellationToken cancellationToken = default);
}
