using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class ExperienceInterestConfiguration : IEntityTypeConfiguration<ExperienceInterest>
{
    public void Configure(EntityTypeBuilder<ExperienceInterest> builder)
    {
        builder.Ignore(x => x.Id);
        builder.HasKey(x => new { x.ExperienceId, x.InterestId });
        builder.HasOne(x => x.Experience).WithMany(x => x.ExperienceInterests).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Interest).WithMany(x => x.ExperienceInterests).HasForeignKey(x => x.InterestId).OnDelete(DeleteBehavior.Restrict);
    }
}

