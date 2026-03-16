using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PowerFuel.Domain.Entities;

namespace PowerFuel.Infrastructure.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);
        builder.Property(ci => ci.UnitPrice).HasPrecision(18, 2);
        builder.ToTable(t => t.HasCheckConstraint("CK_CartItem_Item", "ProductId IS NOT NULL OR EquipmentId IS NOT NULL"));
    }
}
