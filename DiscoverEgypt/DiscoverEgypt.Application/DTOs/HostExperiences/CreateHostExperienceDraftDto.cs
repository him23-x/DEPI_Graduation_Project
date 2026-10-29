

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed class CreateHostExperienceDraftDto
{
    public int ExperienceTypeId { get; set; }
    public int CulturalAreaId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CulturalStory { get; set; } = string.Empty;
    public decimal PricePerPerson { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxGroupSize { get; set; }
    public string MeetingPointArea { get; set; } = string.Empty;
    public string ExactMeetingPoint { get; set; } = string.Empty;
    public IReadOnlyCollection<int> InterestIds { get; set; } = [];
    public IReadOnlyCollection<int> LanguageIds { get; set; } = [];
}
