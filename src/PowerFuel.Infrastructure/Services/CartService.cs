using Microsoft.EntityFrameworkCore;
using PowerFuel.Application.DTOs.Cart;
using PowerFuel.Application.Interfaces;
using CartEntity = PowerFuel.Domain.Entities.Cart;

using PowerFuel.Domain.Entities;
using PowerFuel.Infrastructure.Data;

namespace PowerFuel.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context) => _context = context;

    public async Task<CartDto?> GetCartAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Equipment)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        return cart == null ? null : MapToDto(cart);
    }

    public async Task<CartDto?> AddItemAsync(Guid userId, AddToCartRequest request, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            .Include(c => c.CartItems).ThenInclude(ci => ci.Equipment)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        if (cart == null)
        {
            cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync(cancellationToken);
        }

        decimal unitPrice = 0;
        string? name = null;
        string? imageUrl = null;
        if (request.ItemType.Equals("Product", StringComparison.OrdinalIgnoreCase))
        {
            var product = await _context.Products.FindAsync([request.ItemId], cancellationToken);
            if (product == null) return await GetCartAsync(userId, cancellationToken);
            unitPrice = product.Price;
            name = product.Name;
            imageUrl = product.ImageUrl;
            var existing = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ItemId);
            if (existing != null) { existing.Quantity += request.Quantity; existing.UnitPrice = unitPrice; }
            else cart.CartItems.Add(new CartItem { ProductId = request.ItemId, UnitPrice = unitPrice, Quantity = request.Quantity });
        }
        else if (request.ItemType.Equals("Equipment", StringComparison.OrdinalIgnoreCase))
        {
            var equipment = await _context.Equipments.FindAsync([request.ItemId], cancellationToken);
            if (equipment == null) return await GetCartAsync(userId, cancellationToken);
            unitPrice = equipment.Price;
            name = equipment.Name;
            imageUrl = equipment.ImageUrl;
            var existing = cart.CartItems.FirstOrDefault(ci => ci.EquipmentId == request.ItemId);
            if (existing != null) { existing.Quantity += request.Quantity; existing.UnitPrice = unitPrice; }
            else cart.CartItems.Add(new CartItem { EquipmentId = request.ItemId, UnitPrice = unitPrice, Quantity = request.Quantity });
        }
        else return await GetCartAsync(userId, cancellationToken);

        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return await GetCartAsync(userId, cancellationToken);
    }

    public async Task<CartDto?> UpdateQuantityAsync(Guid userId, int cartItemId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity < 1) return await RemoveItemAsync(userId, cartItemId, cancellationToken) ? await GetCartAsync(userId, cancellationToken) : null;
        var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        var item = cart?.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        if (item == null) return await GetCartAsync(userId, cancellationToken);
        item.Quantity = quantity;
        await _context.SaveChangesAsync(cancellationToken);
        return await GetCartAsync(userId, cancellationToken);
    }

    public async Task<bool> RemoveItemAsync(Guid userId, int cartItemId, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        var item = cart?.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        if (item == null) return false;
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static CartDto MapToDto(Cart cart)
    {
        var items = cart.CartItems.Select(ci =>
        {
            string itemName; string type;
            if (ci.Product != null) { itemName = ci.Product.Name; type = "Product"; }
            else if (ci.Equipment != null) { itemName = ci.Equipment.Name; type = "Equipment"; }
            else { itemName = "Unknown"; type = "Product"; }
            return new CartItemDto(ci.Id, ci.ProductId, ci.EquipmentId, itemName, type, ci.UnitPrice, ci.Quantity, ci.UnitPrice * ci.Quantity, ci.Product?.ImageUrl ?? ci.Equipment?.ImageUrl);
        }).ToList();
        return new CartDto(cart.Id, items, items.Sum(i => i.LineTotal), items.Sum(i => i.Quantity));
    }
}
