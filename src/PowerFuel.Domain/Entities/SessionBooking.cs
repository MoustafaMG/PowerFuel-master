namespace PowerFuel.Domain.Entities;

public class SessionBooking
{
    public int Id { get; set; }
    public int CoachId { get; set; }
    public Guid UserId { get; set; }
    public DateOnly BookingDate { get; set; }
    public TimeOnly BookingTime { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Coach Coach { get; set; } = null!;
    public User User { get; set; } = null!;
}
