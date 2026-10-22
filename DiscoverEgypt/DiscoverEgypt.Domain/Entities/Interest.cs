

namespace DiscoverEgypt.Domain.Entities;

public class Interest : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<PreferenceInterest> PreferenceInterests { get; set; } = new List<PreferenceInterest>();
    public ICollection<ExperienceInterest> ExperienceInterests { get; set; } = new List<ExperienceInterest>();
}

