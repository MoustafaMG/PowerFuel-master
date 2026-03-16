using Microsoft.AspNetCore.Mvc;
using PowerFuel.Application.Interfaces;

namespace PowerFuel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    [HttpGet("products")]
    public async Task<IActionResult> GetProductCategories(CancellationToken cancellationToken)
    {
        var list = await _categoryService.GetProductCategoriesAsync(cancellationToken);
        return Ok(list);
    }

    [HttpGet("equipment")]
    public async Task<IActionResult> GetEquipmentCategories(CancellationToken cancellationToken)
    {
        var list = await _categoryService.GetEquipmentCategoriesAsync(cancellationToken);
        return Ok(list);
    }
}
