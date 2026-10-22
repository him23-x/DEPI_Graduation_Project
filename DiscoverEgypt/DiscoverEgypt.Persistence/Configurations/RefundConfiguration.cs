using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.DecidedByAdminId).HasMaxLength(450);
        builder.HasOne(x => x.Booking).WithMany(x => x.Refunds).HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Dispute).WithMany(x => x.Refunds).HasForeignKey(x => x.DisputeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.DecidedByAdminId).OnDelete(DeleteBehavior.Restrict);
        builder.ToTable(t => t.HasCheckConstraint("CK_Refund_Amount_Positive", "[Amount] > 0"));
    }
}

