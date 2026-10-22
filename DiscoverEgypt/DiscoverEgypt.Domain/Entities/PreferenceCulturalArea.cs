using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class PreferenceCulturalArea : BaseEntity
{
    public int TouristPreferenceId { get; set; }
    public TouristPreference TouristPreference { get; set; } = null!;
    public int CulturalAreaId { get; set; }
    public CulturalArea CulturalArea { get; set; } = null!;
}

