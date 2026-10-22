using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class GuideCulturalAreaConfiguration : IEntityTypeConfiguration<GuideCulturalArea>
{
    public void Configure(EntityTypeBuilder<GuideCulturalArea> builder)
    {
        builder.Ignore(x => x.Id);
        builder.HasKey(x => new { x.GuideProfileId, x.CulturalAreaId });
        builder.HasOne(x => x.GuideProfile).WithMany(x => x.GuideCulturalAreas).HasForeignKey(x => x.GuideProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.CulturalArea).WithMany(x => x.GuideCulturalAreas).HasForeignKey(x => x.CulturalAreaId).OnDelete(DeleteBehavior.Restrict);
    }
}

