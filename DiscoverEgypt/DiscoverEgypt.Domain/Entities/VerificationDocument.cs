using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class VerificationDocument : BaseEntity
{
    public int VerificationRequestId { get; set; }
    public VerificationRequest VerificationRequest { get; set; } = null!;
    public VerificationDocType DocType { get; set; }
    public int? LanguageId { get; set; }
    public Language? Language { get; set; }
    public int? CulturalAreaId { get; set; }
    public CulturalArea? CulturalArea { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

