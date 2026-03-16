namespace PowerFuel.Domain.Entities;

public class EquipmentReview
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    public Equipment Equipment { get; set; } = null!;
    public User User { get; set; } = null!;
}
