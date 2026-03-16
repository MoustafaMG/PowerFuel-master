using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerFuel.Application.DTOs.Cart;
using PowerFuel.Application.Interfaces;

namespace PowerFuel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService) => _cartService = cartService;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var cart = await _cartService.GetCartAsync(UserId, cancellationToken);
        return Ok(cart ?? new CartDto(0, Array.Empty<CartItemDto>(), 0, 0));
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request, CancellationToken cancellationToken)
    {
        var cart = await _cartService.AddItemAsync(UserId, request, cancellationToken);
        return Ok(cart);
    }

    [HttpPut("items/{cartItemId:int}")]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromQuery] int quantity, CancellationToken cancellationToken)
    {
        var cart = await _cartService.UpdateQuantityAsync(UserId, cartItemId, quantity, cancellationToken);
        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpDelete("items/{cartItemId:int}")]
    public async Task<IActionResult> RemoveItem(int cartItemId, CancellationToken cancellationToken)
    {
        var removed = await _cartService.RemoveItemAsync(UserId, cartItemId, cancellationToken);
        if (!removed) return NotFound();
        return NoContent();
    }
}
