using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class ExperienceMediaConfiguration : IEntityTypeConfiguration<ExperienceMedia>
{
    public void Configure(EntityTypeBuilder<ExperienceMedia> builder)
    {
        builder.Property(x => x.MediaType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.StorageKey).IsRequired().HasMaxLength(500);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_ExperienceMedia_DisplayOrder_NonNegative", "[DisplayOrder] >= 0");
            t.HasCheckConstraint("CK_ExperienceMedia_CoverMustBeImage", "[IsCover] = 0 OR [MediaType] = 'Image'");
        });
        builder.HasOne(x => x.Experience).WithMany(x => x.Media).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.ExperienceId, x.DisplayOrder }).IsUnique();
    }
}

