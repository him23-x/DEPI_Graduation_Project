using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class BookingShareConfiguration : IEntityTypeConfiguration<BookingShare>
{
    public void Configure(EntityTypeBuilder<BookingShare> builder)
    {
        builder.Property(x => x.Token).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasOne(x => x.Booking).WithMany(x => x.Shares).HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
    }
}

