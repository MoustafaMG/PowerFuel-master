using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PowerFuel.Domain.Entities;

namespace PowerFuel.Infrastructure.Data.Configurations;

public class EquipmentReviewConfiguration : IEntityTypeConfiguration<EquipmentReview>
{
    public void Configure(EntityTypeBuilder<EquipmentReview> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasOne(r => r.Equipment).WithMany(e => e.EquipmentReviews).HasForeignKey(r => r.EquipmentId);
        builder.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId);
    }
}
