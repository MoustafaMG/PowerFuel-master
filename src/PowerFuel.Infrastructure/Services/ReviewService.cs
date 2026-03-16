using Microsoft.EntityFrameworkCore;
using PowerFuel.Application.DTOs.Reviews;
using PowerFuel.Application.Interfaces;
using PowerFuel.Domain.Entities;
using PowerFuel.Infrastructure.Data;

namespace PowerFuel.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<ReviewDto>> GetProductReviewsAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews.AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto(r.Id, r.Rating, r.Comment, r.User != null ? r.User.UserName : null, r.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReviewDto>> GetEquipmentReviewsAsync(int equipmentId, CancellationToken cancellationToken = default)
    {
        return await _context.EquipmentReviews.AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.EquipmentId == equipmentId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto(r.Id, r.Rating, r.Comment, r.User != null ? r.User.UserName : null, r.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<ReviewDto?> AddProductReviewAsync(int productId, Guid userId, CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Rating is < 1 or > 5) return null;
        var product = await _context.Products.FindAsync([productId], cancellationToken);
        if (product == null) return null;
        var review = new Review { ProductId = productId, UserId = userId, Rating = request.Rating, Comment = request.Comment, CreatedAt = DateTime.UtcNow };
        _context.Reviews.Add(review);
        var reviews = await _context.Reviews.Where(r => r.ProductId == productId).ToListAsync(cancellationToken);
        reviews.Add(review);
        product.AverageRating = (decimal)reviews.Average(r => r.Rating);
        product.ReviewCount = reviews.Count;
        await _context.SaveChangesAsync(cancellationToken);
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        return new ReviewDto(review.Id, review.Rating, review.Comment, user?.UserName, review.CreatedAt);
    }

    public async Task<ReviewDto?> AddEquipmentReviewAsync(int equipmentId, Guid userId, CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Rating is < 1 or > 5) return null;
        var equipment = await _context.Equipments.FindAsync([equipmentId], cancellationToken);
        if (equipment == null) return null;
        var review = new EquipmentReview { EquipmentId = equipmentId, UserId = userId, Rating = request.Rating, Comment = request.Comment, CreatedAt = DateTime.UtcNow };
        _context.EquipmentReviews.Add(review);
        var reviews = await _context.EquipmentReviews.Where(r => r.EquipmentId == equipmentId).ToListAsync(cancellationToken);
        reviews.Add(review);
        equipment.AverageRating = (decimal)reviews.Average(r => r.Rating);
        equipment.ReviewCount = reviews.Count;
        await _context.SaveChangesAsync(cancellationToken);
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        return new ReviewDto(review.Id, review.Rating, review.Comment, user?.UserName, review.CreatedAt);
    }
}
