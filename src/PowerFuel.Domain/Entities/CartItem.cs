namespace PowerFuel.Domain.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public int? ProductId { get; set; }
    public int? EquipmentId { get; set; }

    public Cart Cart { get; set; } = null!;
    public Product? Product { get; set; }
    public Equipment? Equipment { get; set; }
}
