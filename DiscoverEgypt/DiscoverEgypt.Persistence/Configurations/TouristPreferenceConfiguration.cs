using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class TouristPreferenceConfiguration : IEntityTypeConfiguration<TouristPreference>
{
    public void Configure(EntityTypeBuilder<TouristPreference> builder)
    {
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.Property(x => x.Budget).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.Duration).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.GuidePreference).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.PreferenceInterests).WithOne(x => x.TouristPreference).HasForeignKey(x => x.TouristPreferenceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.PreferenceExperienceTypes).WithOne(x => x.TouristPreference).HasForeignKey(x => x.TouristPreferenceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.PreferenceCulturalAreas).WithOne(x => x.TouristPreference).HasForeignKey(x => x.TouristPreferenceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.PreferenceLanguages).WithOne(x => x.TouristPreference).HasForeignKey(x => x.TouristPreferenceId).OnDelete(DeleteBehavior.Cascade);
    }
}

