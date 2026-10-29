using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations.Entities;

public class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
{
    public void Configure(EntityTypeBuilder<Experience> builder)
    {
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(5000);
        builder.Property(x => x.CulturalStory).IsRequired().HasMaxLength(5000);
        builder.Property(x => x.MeetingPointArea).IsRequired().HasMaxLength(300);
        builder.Property(x => x.ExactMeetingPoint).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.PricePerPerson).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.RejectionReason).HasMaxLength(1000);
        builder.Property(x => x.ReviewedByAdminId).HasMaxLength(450);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Experience_PricePerPerson_Positive", "[PricePerPerson] > 0");
            t.HasCheckConstraint("CK_Experience_DurationMinutes_Positive", "[DurationMinutes] > 0");
            t.HasCheckConstraint("CK_Experience_MaxGroupSize_Positive", "[MaxGroupSize] > 0");
        });
        builder.HasOne(x => x.HostProfile).WithMany(x => x.Experiences).HasForeignKey(x => x.HostProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ExperienceType).WithMany(x => x.Experiences).HasForeignKey(x => x.ExperienceTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CulturalArea).WithMany(x => x.Experiences).HasForeignKey(x => x.CulturalAreaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.ReviewedByAdminId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Media).WithOne(x => x.Experience).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Evidence).WithOne(x => x.Experience).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Declarations).WithOne(x => x.Experience).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.ExperienceInterests).WithOne(x => x.Experience).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.ExperienceLanguages).WithOne(x => x.Experience).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Bookings).WithOne(x => x.Experience).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Restrict);
    }
}

