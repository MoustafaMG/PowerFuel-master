namespace PowerFuel.Domain.Entities;

public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? LongDescription { get; set; }
    public string? AdditionalInfo { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public bool IsOnSale { get; set; }
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public decimal? AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int? FeaturedCoachId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Category Category { get; set; } = null!;
    public Coach? FeaturedCoach { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<EquipmentReview> EquipmentReviews { get; set; } = new List<EquipmentReview>();
}
