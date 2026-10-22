

namespace DiscoverEgypt.Domain.Entities;

public class Language : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<GuideLanguage> GuideLanguages { get; set; } = new List<GuideLanguage>();
    public ICollection<VerificationDocument> VerificationDocuments { get; set; } = new List<VerificationDocument>();
    public ICollection<PreferenceLanguage> PreferenceLanguages { get; set; } = new List<PreferenceLanguage>();
    public ICollection<ExperienceLanguage> ExperienceLanguages { get; set; } = new List<ExperienceLanguage>();
}

