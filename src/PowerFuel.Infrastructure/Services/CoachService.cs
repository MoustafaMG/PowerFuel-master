using Microsoft.EntityFrameworkCore;
using PowerFuel.Application.DTOs.Bookings;
using PowerFuel.Application.DTOs.Coaches;
using PowerFuel.Application.Interfaces;
using PowerFuel.Domain.Entities;
using PowerFuel.Infrastructure.Data;

namespace PowerFuel.Infrastructure.Services;

public class CoachService : ICoachService
{
    private readonly ApplicationDbContext _context;

    public CoachService(ApplicationDbContext context) => _context = context;

    public async Task<CoachDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var c = await _context.Coaches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return c == null ? null : MapToDto(c);
    }

    public async Task<IReadOnlyList<CoachListDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Coaches.AsNoTracking()
            .Select(c => new CoachListDto(c.Id, c.FirstName + " " + c.LastName, c.Title, c.Specialization, c.ProfilePictureUrl))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CoachAvailabilityDto>> GetAvailabilityAsync(int coachId, CancellationToken cancellationToken = default)
    {
        return await _context.CoachAvailabilities.AsNoTracking()
            .Where(a => a.CoachId == coachId)
            .OrderBy(a => a.DayOfWeek)
            .Select(a => new CoachAvailabilityDto(a.Id, a.DayOfWeek.ToString(), a.StartTime.ToString("hhtt"), a.EndTime.ToString("hhtt")))
            .ToListAsync(cancellationToken);
    }

    public async Task<BookingDto?> CreateBookingAsync(int coachId, Guid userId, CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        var coach = await _context.Coaches.FindAsync([coachId], cancellationToken);
        if (coach == null) return null;
        var availability = await _context.CoachAvailabilities
            .AnyAsync(a => a.CoachId == coachId && a.DayOfWeek == request.BookingDate.DayOfWeek &&
                           request.BookingTime >= a.StartTime && request.BookingTime < a.EndTime, cancellationToken);
        if (!availability) return null;
        var conflict = await _context.SessionBookings
            .AnyAsync(b => b.CoachId == coachId && b.BookingDate == request.BookingDate &&
                           b.BookingTime == request.BookingTime && b.Status != "Cancelled", cancellationToken);
        if (conflict) return null;

        var booking = new SessionBooking
        {
            CoachId = coachId,
            UserId = userId,
            BookingDate = request.BookingDate,
            BookingTime = request.BookingTime,
            Status = "Confirmed",
            CreatedAt = DateTime.UtcNow
        };
        _context.SessionBookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);
        var coachName = coach.FirstName + " " + coach.LastName;
        return new BookingDto(booking.Id, coachId, coachName, request.BookingDate, request.BookingTime, booking.Status, booking.CreatedAt);
    }

    private static CoachDto MapToDto(Coach c) => new(
        c.Id,
        c.FirstName,
        c.LastName,
        c.FirstName + " " + c.LastName,
        c.Title,
        c.Specialization,
        c.ProfilePictureUrl,
        c.AboutDescription,
        c.YearsExperience,
        c.HappyClientsCount,
        c.CertificationsCount,
        c.PhoneNumber,
        c.Email,
        c.HourlyRate
    );
}
