using Microsoft.AspNetCore.Mvc;
using PowerFuel.Application.Interfaces;

namespace PowerFuel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentsController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentsController(IEquipmentService equipmentService) => _equipmentService = equipmentService;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var equipment = await _equipmentService.GetByIdAsync(id, cancellationToken);
        if (equipment == null) return NotFound();
        return Ok(equipment);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? category, CancellationToken cancellationToken)
    {
        var list = await _equipmentService.ListAsync(category, cancellationToken);
        return Ok(list);
    }
}
