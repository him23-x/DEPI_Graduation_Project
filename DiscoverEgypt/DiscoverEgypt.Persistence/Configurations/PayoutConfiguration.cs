using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class PayoutConfiguration : IEntityTypeConfiguration<Payout>
{
    public void Configure(EntityTypeBuilder<Payout> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.HasIndex(x => x.BookingId).IsUnique();
        builder.HasOne(x => x.Booking).WithOne(x => x.Payout).HasForeignKey<Payout>(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.ToTable(t => t.HasCheckConstraint("CK_Payout_Amount_NonNegative", "[Amount] >= 0"));
    }
}

