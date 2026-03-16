namespace PowerFuel.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    /// <summary> "Product" for store (Supplements, etc.), "Equipment" for gym equipment (Chest, Back, etc.). </summary>
    public string Kind { get; set; } = "Product";

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
}
