using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class ExperienceLanguageConfiguration : IEntityTypeConfiguration<ExperienceLanguage>
{
    public void Configure(EntityTypeBuilder<ExperienceLanguage> builder)
    {
        builder.Ignore(x => x.Id);
        builder.HasKey(x => new { x.ExperienceId, x.LanguageId });
        builder.HasOne(x => x.Experience).WithMany(x => x.ExperienceLanguages).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Language).WithMany(x => x.ExperienceLanguages).HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
    }
}

