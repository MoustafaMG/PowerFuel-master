using Microsoft.EntityFrameworkCore;
using PowerFuel.Application.DTOs.Orders;
using PowerFuel.Application.Interfaces;
using OrderEntity = PowerFuel.Domain.Entities.Order;

using PowerFuel.Domain.Entities;
using PowerFuel.Infrastructure.Data;

namespace PowerFuel.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private const decimal FreeShippingThreshold = 50m;
    private const decimal DefaultShippingCost = 5.99m;

    public OrderService(ApplicationDbContext context) => _context = context;

    public async Task<OrderDto?> CreateOrderFromCartAsync(Guid userId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            .Include(c => c.CartItems).ThenInclude(ci => ci.Equipment)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        if (cart == null || !cart.CartItems.Any()) return null;

        var subTotal = 0m;
        var orderItems = new List<OrderItem>();
        foreach (var ci in cart.CartItems)
        {
            if (ci.ProductId.HasValue && ci.Product != null)
            {
                subTotal += ci.UnitPrice * ci.Quantity;
                orderItems.Add(new OrderItem { ProductId = ci.ProductId, UnitPrice = ci.UnitPrice, Quantity = ci.Quantity, ItemType = "Product" });
            }
            else if (ci.EquipmentId.HasValue && ci.Equipment != null)
            {
                subTotal += ci.UnitPrice * ci.Quantity;
                orderItems.Add(new OrderItem { EquipmentId = ci.EquipmentId, UnitPrice = ci.UnitPrice, Quantity = ci.Quantity, ItemType = "Equipment" });
            }
        }
        var shippingCost = subTotal >= FreeShippingThreshold ? 0 : DefaultShippingCost;
        var order = new Order
        {
            UserId = userId,
            Status = "Pending",
            SubTotal = subTotal,
            ShippingCost = shippingCost,
            TotalAmount = subTotal + shippingCost,
            ShippingAddress = request.ShippingAddress,
            OrderDate = DateTime.UtcNow
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);
        foreach (var oi in orderItems) oi.OrderId = order.Id;
        _context.OrderItems.AddRange(orderItems);
        _context.CartItems.RemoveRange(cart.CartItems);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(order.Id, userId, cancellationToken);
    }

    public async Task<OrderDto?> GetByIdAsync(int orderId, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders.AsNoTracking()
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Equipment)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId, cancellationToken);
        return order == null ? null : MapToDto(order);
    }

    public async Task<IReadOnlyList<OrderDto>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders.AsNoTracking()
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Equipment)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => MapToDto(o))
            .ToListAsync(cancellationToken);
    }

    private static OrderDto MapToDto(Domain.Entities.Order o)
    {
        var items = o.OrderItems.Select(oi => new OrderItemDto(
            oi.Id,
            oi.Product?.Name ?? oi.Equipment?.Name ?? "Unknown",
            oi.ItemType,
            oi.UnitPrice,
            oi.Quantity,
            oi.UnitPrice * oi.Quantity
        )).ToList();
        return new OrderDto(o.Id, o.Status, o.SubTotal, o.ShippingCost, o.TotalAmount, o.ShippingAddress, o.OrderDate, items);
    }
}
