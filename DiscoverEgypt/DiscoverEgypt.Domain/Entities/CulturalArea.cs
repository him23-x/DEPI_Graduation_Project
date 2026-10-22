

namespace DiscoverEgypt.Domain.Entities;

public class CulturalArea : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<GuideCulturalArea> GuideCulturalAreas { get; set; } = new List<GuideCulturalArea>();
    public ICollection<VerificationDocument> VerificationDocuments { get; set; } = new List<VerificationDocument>();
    public ICollection<PreferenceCulturalArea> PreferenceCulturalAreas { get; set; } = new List<PreferenceCulturalArea>();
    public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
}

