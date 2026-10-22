using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class PreferenceExperienceType : BaseEntity
{
    public int TouristPreferenceId { get; set; }
    public TouristPreference TouristPreference { get; set; } = null!;
    public int ExperienceTypeId { get; set; }
    public ExperienceType ExperienceType { get; set; } = null!;
}

