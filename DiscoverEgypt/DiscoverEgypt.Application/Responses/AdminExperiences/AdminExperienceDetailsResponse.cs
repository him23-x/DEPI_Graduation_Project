using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Responses.AdminExperiences
{
    public sealed record AdminExperienceDetailsResponse(
        int ExperienceId,
        string Title,
        string ExperienceType,
        string CulturalArea,
        string HostName,
        string Description,
        string CulturalStory,
        decimal PricePerPerson,
        int DurationMinutes,
        int MaxGroupSize,
        string MeetingPointArea,
        string ExactMeetingPoint,
        DateTime SubmittedAt,
        IReadOnlyList<string> Interests,
        IReadOnlyList<string> Languages,
        int ImageCount,
        int VideoCount,
        bool HasCoverImage,
        IReadOnlyList<AdminExperienceEvidenceResponse> Evidence,
        IReadOnlyList<AdminExperienceDeclarationResponse> Declarations,
        ExperienceStatus Status,
        string? RejectionReason,
        int CoverImageCount);
}
