using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed class CreateExperienceDto
{
    public int ExperienceId { get; set; }
    public ExperienceStatus? Status { get; set; }
    public string? RejectionReason { get; set; }
    public bool IsEditing => ExperienceId > 0;
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
    public int[] InterestIds { get; set; } = [];
    public int[] LanguageIds { get; set; } = [];
    public HostExperienceCreateOptions Options { get; set; } = new([], [], [], []);
    public bool HasRequiredLookups => Options.ExperienceTypes.Count > 0
        && Options.CulturalAreas.Count > 0
        && Options.Interests.Count > 0
        && Options.Languages.Count > 0;
}

