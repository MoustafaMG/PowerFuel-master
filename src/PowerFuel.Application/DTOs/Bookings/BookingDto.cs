namespace PowerFuel.Application.DTOs.Bookings;

public record BookingDto(
    int Id,
    int CoachId,
    string CoachName,
    DateOnly BookingDate,
    TimeOnly BookingTime,
    string Status,
    DateTime CreatedAt
);
