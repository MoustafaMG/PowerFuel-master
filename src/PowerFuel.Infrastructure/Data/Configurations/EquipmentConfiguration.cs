using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PowerFuel.Domain.Entities;

namespace PowerFuel.Infrastructure.Data.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Price).HasPrecision(18, 2);
        builder.Property(e => e.OriginalPrice).HasPrecision(18, 2);
        builder.Property(e => e.AverageRating).HasPrecision(3, 2);
        builder.HasOne(e => e.Category).WithMany(c => c.Equipments).HasForeignKey(e => e.CategoryId);
        builder.HasOne(e => e.FeaturedCoach).WithMany(c => c.FeaturedEquipments)
            .HasForeignKey(e => e.FeaturedCoachId).OnDelete(DeleteBehavior.SetNull);
    }
}
