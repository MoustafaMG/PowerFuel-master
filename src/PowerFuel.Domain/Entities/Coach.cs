namespace PowerFuel.Domain.Entities;

public class Coach
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? AboutDescription { get; set; }
    public int YearsExperience { get; set; }
    public int HappyClientsCount { get; set; }
    public int CertificationsCount { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<CoachAvailability> Availabilities { get; set; } = new List<CoachAvailability>();
    public ICollection<SessionBooking> SessionBookings { get; set; } = new List<SessionBooking>();
    public ICollection<Equipment> FeaturedEquipments { get; set; } = new List<Equipment>();
}
