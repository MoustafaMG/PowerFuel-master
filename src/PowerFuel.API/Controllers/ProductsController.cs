using Microsoft.AspNetCore.Mvc;
using PowerFuel.Application.Interfaces;

namespace PowerFuel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService) => _productService = productService;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet("best-sellers")]
    public async Task<IActionResult> GetBestSellers([FromQuery] int count = 8, CancellationToken cancellationToken = default)
    {
        var list = await _productService.GetBestSellersAsync(count, cancellationToken);
        return Ok(list);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? category, [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var list = await _productService.ListAsync(category, search, cancellationToken);
        return Ok(list);
    }
}
