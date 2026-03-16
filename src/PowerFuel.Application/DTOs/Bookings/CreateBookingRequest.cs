namespace PowerFuel.Application.DTOs.Bookings;

public record CreateBookingRequest(int CoachId, DateOnly BookingDate, TimeOnly BookingTime);
