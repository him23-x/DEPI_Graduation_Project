
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Domain.Entities;

public class ExperienceEvidence : BaseEntity
{
    public int ExperienceId { get; set; }
    public Experience Experience { get; set; } = null!;
    public EvidenceKind EvidenceKind { get; set; }
    public string? TextContent { get; set; }
    public string? StorageKey { get; set; }
    public DateTime UploadedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

