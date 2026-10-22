using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class ExperienceMedia : BaseEntity
{
    public int ExperienceId { get; set; }
    public Experience Experience { get; set; } = null!;
    public MediaType MediaType { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsCover { get; set; }
    public DateTime CreatedAt { get; set; }
}

