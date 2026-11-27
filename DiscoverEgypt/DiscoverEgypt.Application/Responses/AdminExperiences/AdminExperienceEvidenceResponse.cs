using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Responses.AdminExperiences
{
    public sealed record AdminExperienceEvidenceResponse(
        int EvidenceId,
        EvidenceKind Kind,
        string? TextContent,
        DateTime UploadedAt,
        bool HasFile);
}
