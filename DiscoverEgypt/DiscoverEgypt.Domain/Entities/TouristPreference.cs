using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class TouristPreference : BaseEntity
{
    public string UserId { get; set; }
    public Budget Budget { get; set; }
    public DurationPreference Duration { get; set; }
    public GuidePreference GuidePreference { get; set; }
    public ICollection<PreferenceInterest> PreferenceInterests { get; set; } = new List<PreferenceInterest>();
    public ICollection<PreferenceExperienceType> PreferenceExperienceTypes { get; set; } = new List<PreferenceExperienceType>();
    public ICollection<PreferenceCulturalArea> PreferenceCulturalAreas { get; set; } = new List<PreferenceCulturalArea>();
    public ICollection<PreferenceLanguage> PreferenceLanguages { get; set; } = new List<PreferenceLanguage>();
}

