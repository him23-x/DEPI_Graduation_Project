using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class PreferenceInterestConfiguration : IEntityTypeConfiguration<PreferenceInterest>
{
    public void Configure(EntityTypeBuilder<PreferenceInterest> builder)
    {
        builder.Ignore(x => x.Id);
        builder.HasKey(x => new { x.TouristPreferenceId, x.InterestId });
        builder.HasOne(x => x.TouristPreference).WithMany(x => x.PreferenceInterests).HasForeignKey(x => x.TouristPreferenceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Interest).WithMany(x => x.PreferenceInterests).HasForeignKey(x => x.InterestId).OnDelete(DeleteBehavior.Restrict);
    }
}

