using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(x => x.Comment).HasMaxLength(2000);
        builder.HasIndex(x => x.BookingId).IsUnique();
        builder.HasOne(x => x.Booking).WithOne(x => x.Review).HasForeignKey<Review>(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.ToTable(t => t.HasCheckConstraint("CK_Review_Rating_Range", "[Rating] >= 1 AND [Rating] <= 5"));
    }
}

