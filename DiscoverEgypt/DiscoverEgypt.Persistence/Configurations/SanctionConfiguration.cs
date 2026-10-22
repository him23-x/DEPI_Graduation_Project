using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class SanctionConfiguration : IEntityTypeConfiguration<Sanction>
{
    public void Configure(EntityTypeBuilder<Sanction> builder)
    {
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.SanctionType).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.DecidedByAdminId).IsRequired().HasMaxLength(450);
        builder.HasOne(x => x.Dispute).WithMany(x => x.Sanctions).HasForeignKey(x => x.DisputeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.DecidedByAdminId).OnDelete(DeleteBehavior.Restrict);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Sanction_Days_Positive", "[Days] > 0");
            t.HasCheckConstraint("CK_Sanction_ValidPeriod", "[EndsAt] > [StartsAt]");
        });
    }
}

