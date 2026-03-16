namespace PowerFuel.Domain.Entities;

public class CoachAvailability
{
    public int Id { get; set; }
    public int CoachId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public Coach Coach { get; set; } = null!;
}
