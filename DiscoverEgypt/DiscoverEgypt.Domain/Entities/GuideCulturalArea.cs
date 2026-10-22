using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class GuideCulturalArea : BaseEntity
{
    public int GuideProfileId { get; set; }
    public GuideProfile GuideProfile { get; set; } = null!;
    public int CulturalAreaId { get; set; }
    public CulturalArea CulturalArea { get; set; } = null!;
}

