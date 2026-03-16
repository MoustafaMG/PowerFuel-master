using PowerFuel.Application.DTOs.Orders;

namespace PowerFuel.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto?> CreateOrderFromCartAsync(Guid userId, CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderDto?> GetByIdAsync(int orderId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderDto>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken = default);
}
