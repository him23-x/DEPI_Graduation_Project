using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class PreferenceLanguageConfiguration : IEntityTypeConfiguration<PreferenceLanguage>
{
    public void Configure(EntityTypeBuilder<PreferenceLanguage> builder)
    {
        builder.Ignore(x => x.Id);
        builder.HasKey(x => new { x.TouristPreferenceId, x.LanguageId });
        builder.HasOne(x => x.TouristPreference).WithMany(x => x.PreferenceLanguages).HasForeignKey(x => x.TouristPreferenceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Language).WithMany(x => x.PreferenceLanguages).HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
    }
}

