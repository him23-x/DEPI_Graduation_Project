using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class PreferenceExperienceTypeConfiguration : IEntityTypeConfiguration<PreferenceExperienceType>
{
    public void Configure(EntityTypeBuilder<PreferenceExperienceType> builder)
    {
        builder.Ignore(x => x.Id);
        builder.HasKey(x => new { x.TouristPreferenceId, x.ExperienceTypeId });
        builder.HasOne(x => x.TouristPreference).WithMany(x => x.PreferenceExperienceTypes).HasForeignKey(x => x.TouristPreferenceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ExperienceType).WithMany(x => x.PreferenceExperienceTypes).HasForeignKey(x => x.ExperienceTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}

