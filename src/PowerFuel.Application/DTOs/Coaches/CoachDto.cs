namespace PowerFuel.Application.DTOs.Coaches;

public record CoachDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    string Title,
    string Specialization,
    string? ProfilePictureUrl,
    string? AboutDescription,
    int YearsExperience,
    int HappyClientsCount,
    int CertificationsCount,
    string? PhoneNumber,
    string? Email,
    decimal HourlyRate
);
