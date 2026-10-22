using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class DisputeConfiguration : IEntityTypeConfiguration<Dispute>
{
    public void Configure(EntityTypeBuilder<Dispute> builder)
    {
        builder.Property(x => x.OpenedByUserId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(5000);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.Outcome).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.ResolutionNote).HasMaxLength(3000);
        builder.Property(x => x.ResolvedByAdminId).HasMaxLength(450);
        builder.HasOne(x => x.Booking).WithMany(x => x.Disputes).HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.OpenedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.ResolvedByAdminId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Refunds).WithOne(x => x.Dispute).HasForeignKey(x => x.DisputeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Sanctions).WithOne(x => x.Dispute).HasForeignKey(x => x.DisputeId).OnDelete(DeleteBehavior.Cascade);
    }
}

