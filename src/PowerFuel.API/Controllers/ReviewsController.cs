using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerFuel.Application.DTOs.Reviews;
using PowerFuel.Application.Interfaces;

namespace PowerFuel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService) => _reviewService = reviewService;

    [HttpGet("products/{productId:int}")]
    public async Task<IActionResult> GetProductReviews(int productId, CancellationToken cancellationToken)
    {
        var list = await _reviewService.GetProductReviewsAsync(productId, cancellationToken);
        return Ok(list);
    }

    [HttpGet("equipment/{equipmentId:int}")]
    public async Task<IActionResult> GetEquipmentReviews(int equipmentId, CancellationToken cancellationToken)
    {
        var list = await _reviewService.GetEquipmentReviewsAsync(equipmentId, cancellationToken);
        return Ok(list);
    }

    [HttpPost("products/{productId:int}")]
    [Authorize]
    public async Task<IActionResult> AddProductReview(int productId, [FromBody] CreateReviewRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        var review = await _reviewService.AddProductReviewAsync(productId, userId, request, cancellationToken);
        if (review == null) return BadRequest(new { message = "Invalid rating or product not found." });
        return CreatedAtAction(nameof(GetProductReviews), new { productId }, review);
    }

    [HttpPost("equipment/{equipmentId:int}")]
    [Authorize]
    public async Task<IActionResult> AddEquipmentReview(int equipmentId, [FromBody] CreateReviewRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        var review = await _reviewService.AddEquipmentReviewAsync(equipmentId, userId, request, cancellationToken);
        if (review == null) return BadRequest(new { message = "Invalid rating or equipment not found." });
        return CreatedAtAction(nameof(GetEquipmentReviews), new { equipmentId }, review);
    }
}
