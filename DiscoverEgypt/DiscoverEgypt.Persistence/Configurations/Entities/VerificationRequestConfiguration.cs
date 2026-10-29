using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations.Entities;

public class VerificationRequestConfiguration : IEntityTypeConfiguration<VerificationRequest>
{
    public void Configure(EntityTypeBuilder<VerificationRequest> builder)
    {
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.LicenseExpiryDate).IsRequired();
        builder.Property(x => x.RejectionReason).HasMaxLength(1000);
        builder.Property(x => x.ReviewedByAdminId).HasMaxLength(450);
        builder.ToTable(t => t.HasCheckConstraint("CK_VerificationRequest_ReviewFields", "([ReviewedByAdminId] IS NULL AND [ReviewedAt] IS NULL) OR ([ReviewedByAdminId] IS NOT NULL AND [ReviewedAt] IS NOT NULL)"));
        builder.HasOne(x => x.GuideProfile).WithMany(x => x.VerificationRequests).HasForeignKey(x => x.GuideProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.ReviewedByAdminId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Documents).WithOne(x => x.VerificationRequest).HasForeignKey(x => x.VerificationRequestId).OnDelete(DeleteBehavior.Cascade);
    }
}

