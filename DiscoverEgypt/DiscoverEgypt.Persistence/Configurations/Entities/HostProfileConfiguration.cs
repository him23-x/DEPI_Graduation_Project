using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DiscoverEgypt.Domain.Enums.Hosts;

namespace DiscoverEgypt.Persistence.Configurations.Entities;

public class HostProfileConfiguration : IEntityTypeConfiguration<HostProfile>
{
    public void Configure(EntityTypeBuilder<HostProfile> builder)
    {
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.Property(x => x.HostType).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.Bio).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.ProfileImageKey).HasMaxLength(500);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Experiences).WithOne(x => x.HostProfile).HasForeignKey(x => x.HostProfileId).OnDelete(DeleteBehavior.Restrict);
    }
}

