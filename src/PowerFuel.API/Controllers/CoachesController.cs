using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerFuel.Application.DTOs.Bookings;
using PowerFuel.Application.Interfaces;

namespace PowerFuel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoachesController : ControllerBase
{
    private readonly ICoachService _coachService;

    public CoachesController(ICoachService coachService) => _coachService = coachService;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var coach = await _coachService.GetByIdAsync(id, cancellationToken);
        if (coach == null) return NotFound();
        return Ok(coach);
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var list = await _coachService.ListAsync(cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:int}/availability")]
    public async Task<IActionResult> GetAvailability(int id, CancellationToken cancellationToken)
    {
        var list = await _coachService.GetAvailabilityAsync(id, cancellationToken);
        return Ok(list);
    }

    [HttpPost("{id:int}/bookings")]
    [Authorize]
    public async Task<IActionResult> CreateBooking(int id, [FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        var booking = await _coachService.CreateBookingAsync(id, userId, request, cancellationToken);
        if (booking == null) return BadRequest(new { message = "Coach not found, slot not available, or time conflict." });
        return CreatedAtAction(nameof(GetById), new { id }, booking);
    }
}
