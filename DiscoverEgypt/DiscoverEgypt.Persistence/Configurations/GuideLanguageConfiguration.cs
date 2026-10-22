using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class GuideLanguageConfiguration : IEntityTypeConfiguration<GuideLanguage>
{
    public void Configure(EntityTypeBuilder<GuideLanguage> builder)
    {
        builder.Ignore(x => x.Id);
        builder.HasKey(x => new { x.GuideProfileId, x.LanguageId });
        builder.HasOne(x => x.GuideProfile).WithMany(x => x.GuideLanguages).HasForeignKey(x => x.GuideProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Language).WithMany(x => x.GuideLanguages).HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
    }
}

