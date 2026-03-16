using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PowerFuel.Domain.Entities;

namespace PowerFuel.Infrastructure.Data.Configurations;

public class CoachAvailabilityConfiguration : IEntityTypeConfiguration<CoachAvailability>
{
    public void Configure(EntityTypeBuilder<CoachAvailability> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasOne(a => a.Coach).WithMany(c => c.Availabilities).HasForeignKey(a => a.CoachId);
    }
}
