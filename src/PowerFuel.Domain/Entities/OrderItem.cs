namespace PowerFuel.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string ItemType { get; set; } = "Product"; // Product, Equipment

    public int? ProductId { get; set; }
    public int? EquipmentId { get; set; }

    public Order Order { get; set; } = null!;
    public Product? Product { get; set; }
    public Equipment? Equipment { get; set; }
}
