using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Persistence.Configurations.Entities;

public class ExperienceEvidenceConfiguration : IEntityTypeConfiguration<ExperienceEvidence>
{
    public void Configure(EntityTypeBuilder<ExperienceEvidence> builder)
    {
        builder.Property(x => x.EvidenceKind).HasConversion<string>().HasMaxLength(40).IsRequired();
        builder.Property(x => x.TextContent).HasMaxLength(5000);
        builder.Property(x => x.StorageKey).HasMaxLength(500);
        builder.ToTable(t => t.HasCheckConstraint("CK_ExperienceEvidence_HasContent", "[TextContent] IS NOT NULL OR [StorageKey] IS NOT NULL"));
        builder.HasOne(x => x.Experience).WithMany(x => x.Evidence).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Cascade);
    }
}

