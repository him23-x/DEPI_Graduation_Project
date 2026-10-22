using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class ExperienceLanguage : BaseEntity
{
    public int ExperienceId { get; set; }
    public Experience Experience { get; set; } = null!;
    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;
}

