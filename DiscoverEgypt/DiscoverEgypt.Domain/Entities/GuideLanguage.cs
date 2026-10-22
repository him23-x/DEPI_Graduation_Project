using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class GuideLanguage : BaseEntity
{
    public int GuideProfileId { get; set; }
    public GuideProfile GuideProfile { get; set; } = null!;
    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;
    public bool IsNative { get; set; }
}

