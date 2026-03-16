using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PowerFuel.Domain.Entities;

namespace PowerFuel.Infrastructure.Data.Configurations;

public class SessionBookingConfiguration : IEntityTypeConfiguration<SessionBooking>
{
    public void Configure(EntityTypeBuilder<SessionBooking> builder)
    {
        builder.HasKey(b => b.Id);
        builder.HasOne(b => b.Coach).WithMany(c => c.SessionBookings).HasForeignKey(b => b.CoachId);
        builder.HasOne(b => b.User).WithMany(u => u.SessionBookings).HasForeignKey(b => b.UserId);
    }
}
