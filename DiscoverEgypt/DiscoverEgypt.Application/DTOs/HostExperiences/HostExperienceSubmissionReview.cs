using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceSubmissionReview(
    int ExperienceId,
    string Title,
    string ExperienceTypeName,
    EvidenceExperienceType ExperienceType,
    string CulturalArea,
    string Description,
    string CulturalStory,
    decimal PricePerPerson,
    int DurationMinutes,
    int MaxGroupSize,
    string MeetingPointArea,
    string ExactMeetingPoint,
    ExperienceStatus Status,
    string? RejectionReason,
    int ImageCount,
    int VideoCount,
    bool HasCoverImage,
    IReadOnlyList<HostExperienceEvidenceSummary> Evidence,
    bool HasInterest,
    bool HasLanguage,
    IReadOnlyList<string> Issues);

