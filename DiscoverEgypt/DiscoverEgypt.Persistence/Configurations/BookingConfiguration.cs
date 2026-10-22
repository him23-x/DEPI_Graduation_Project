using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(x => x.TouristUserId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.ProviderUserId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.BookingType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Destination).HasMaxLength(300);
        builder.Property(x => x.PricePerPersonSnapshot).HasPrecision(18, 2);
        builder.Property(x => x.HourlyRateSnapshot).HasPrecision(18, 2);
        builder.Property(x => x.ServiceAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.PlatformFeeRate).HasPrecision(9, 6).IsRequired();
        builder.Property(x => x.PlatformFeeAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Booking_TotalAmount_NonNegative", "[TotalAmount] >= 0");
            t.HasCheckConstraint("CK_Booking_ServiceAmount_NonNegative", "[ServiceAmount] >= 0");
            t.HasCheckConstraint("CK_Booking_PlatformFeeRate_Range", "[PlatformFeeRate] >= 0 AND [PlatformFeeRate] <= 1");
            t.HasCheckConstraint("CK_Booking_ScheduledPeriod", "[ScheduledEnd] > [ScheduledStart]");
            t.HasCheckConstraint("CK_Booking_ValidTypeTarget", "([BookingType] = 'Experience' AND [ExperienceId] IS NOT NULL AND [GuideProfileId] IS NULL AND [NumberOfGuests] > 0 AND [GuideHours] IS NULL AND [Destination] IS NULL AND [PricePerPersonSnapshot] IS NOT NULL AND [HourlyRateSnapshot] IS NULL) OR ([BookingType] = 'Guide' AND [GuideProfileId] IS NOT NULL AND [ExperienceId] IS NULL AND [NumberOfGuests] IS NULL AND [GuideHours] > 0 AND [PricePerPersonSnapshot] IS NULL AND [HourlyRateSnapshot] IS NOT NULL)");
        });
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.TouristUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.ProviderUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Experience).WithMany(x => x.Bookings).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.GuideProfile).WithMany(x => x.Bookings).HasForeignKey(x => x.GuideProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Payment).WithOne(x => x.Booking).HasForeignKey<Payment>(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Payout).WithOne(x => x.Booking).HasForeignKey<Payout>(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Review).WithOne(x => x.Booking).HasForeignKey<Review>(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Refunds).WithOne(x => x.Booking).HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Disputes).WithOne(x => x.Booking).HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Shares).WithOne(x => x.Booking).HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Notifications).WithOne(x => x.Booking).HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.SetNull);
    }
}

