using PowerFuel.Application.DTOs.Reviews;

namespace PowerFuel.Application.Interfaces;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewDto>> GetProductReviewsAsync(int productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReviewDto>> GetEquipmentReviewsAsync(int equipmentId, CancellationToken cancellationToken = default);
    Task<ReviewDto?> AddProductReviewAsync(int productId, Guid userId, CreateReviewRequest request, CancellationToken cancellationToken = default);
    Task<ReviewDto?> AddEquipmentReviewAsync(int equipmentId, Guid userId, CreateReviewRequest request, CancellationToken cancellationToken = default);
}
