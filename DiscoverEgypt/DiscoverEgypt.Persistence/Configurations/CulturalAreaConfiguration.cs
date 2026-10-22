using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class CulturalAreaConfiguration : IEntityTypeConfiguration<CulturalArea>
{
    public void Configure(EntityTypeBuilder<CulturalArea> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}

