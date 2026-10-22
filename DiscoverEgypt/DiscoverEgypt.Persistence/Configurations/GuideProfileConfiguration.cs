using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class GuideProfileConfiguration : IEntityTypeConfiguration<GuideProfile>
{
    public void Configure(EntityTypeBuilder<GuideProfile> builder)
    {
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.Property(x => x.Bio).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.HourlyRate).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.ProfileImageKey).HasMaxLength(500);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.VerificationRequests).WithOne(x => x.GuideProfile).HasForeignKey(x => x.GuideProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.GuideLanguages).WithOne(x => x.GuideProfile).HasForeignKey(x => x.GuideProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.GuideCulturalAreas).WithOne(x => x.GuideProfile).HasForeignKey(x => x.GuideProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Bookings).WithOne(x => x.GuideProfile).HasForeignKey(x => x.GuideProfileId).OnDelete(DeleteBehavior.Restrict);
    }
}

