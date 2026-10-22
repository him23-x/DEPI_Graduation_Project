using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class PreferenceLanguage : BaseEntity
{
    public int TouristPreferenceId { get; set; }
    public TouristPreference TouristPreference { get; set; } = null!;
    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;
}

