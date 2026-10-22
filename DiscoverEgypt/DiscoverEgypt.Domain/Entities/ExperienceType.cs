

namespace DiscoverEgypt.Domain.Entities;

public class ExperienceType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<PreferenceExperienceType> PreferenceExperienceTypes { get; set; } = new List<PreferenceExperienceType>();
    public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
}

