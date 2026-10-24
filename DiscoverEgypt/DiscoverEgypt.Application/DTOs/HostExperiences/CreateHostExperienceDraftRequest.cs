namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record CreateHostExperienceDraftRequest(
    int ExperienceTypeId,
    int CulturalAreaId,
    string Title,
    string Description,
    string CulturalStory,
    decimal PricePerPerson,
    int DurationMinutes,
    int MaxGroupSize,
    string MeetingPointArea,
    string ExactMeetingPoint,
    IReadOnlyCollection<int> InterestIds,
    IReadOnlyCollection<int> LanguageIds);
