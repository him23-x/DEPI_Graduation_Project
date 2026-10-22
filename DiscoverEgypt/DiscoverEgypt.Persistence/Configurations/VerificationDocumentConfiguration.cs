using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class VerificationDocumentConfiguration : IEntityTypeConfiguration<VerificationDocument>
{
    public void Configure(EntityTypeBuilder<VerificationDocument> builder)
    {
        builder.Property(x => x.DocType).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.StorageKey).IsRequired().HasMaxLength(500);
        builder.ToTable(t => t.HasCheckConstraint("CK_VerificationDocument_TypeReferences", "([DocType] = 'Language' AND [LanguageId] IS NOT NULL AND [CulturalAreaId] IS NULL) OR ([DocType] = 'Residence' AND [CulturalAreaId] IS NOT NULL AND [LanguageId] IS NULL) OR ([DocType] NOT IN ('Language', 'Residence') AND [LanguageId] IS NULL AND [CulturalAreaId] IS NULL)"));
        builder.HasOne(x => x.VerificationRequest).WithMany(x => x.Documents).HasForeignKey(x => x.VerificationRequestId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Language).WithMany(x => x.VerificationDocuments).HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CulturalArea).WithMany(x => x.VerificationDocuments).HasForeignKey(x => x.CulturalAreaId).OnDelete(DeleteBehavior.Restrict);
    }
}

