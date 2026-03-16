using PowerFuel.Application.DTOs.Coaches;
using PowerFuel.Application.DTOs.Bookings;

namespace PowerFuel.Application.Interfaces;

public interface ICoachService
{
    Task<CoachDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CoachListDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CoachAvailabilityDto>> GetAvailabilityAsync(int coachId, CancellationToken cancellationToken = default);
    Task<BookingDto?> CreateBookingAsync(int coachId, Guid userId, CreateBookingRequest request, CancellationToken cancellationToken = default);
}
