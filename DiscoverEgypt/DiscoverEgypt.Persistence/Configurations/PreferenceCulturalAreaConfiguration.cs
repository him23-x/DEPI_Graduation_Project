using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class PreferenceCulturalAreaConfiguration : IEntityTypeConfiguration<PreferenceCulturalArea>
{
    public void Configure(EntityTypeBuilder<PreferenceCulturalArea> builder)
    {
        builder.Ignore(x => x.Id);
        builder.HasKey(x => new { x.TouristPreferenceId, x.CulturalAreaId });
        builder.HasOne(x => x.TouristPreference).WithMany(x => x.PreferenceCulturalAreas).HasForeignKey(x => x.TouristPreferenceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.CulturalArea).WithMany(x => x.PreferenceCulturalAreas).HasForeignKey(x => x.CulturalAreaId).OnDelete(DeleteBehavior.Restrict);
    }
}

