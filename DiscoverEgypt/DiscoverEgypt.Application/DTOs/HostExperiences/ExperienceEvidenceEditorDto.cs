
using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences
{
    public sealed record ExperienceEvidenceEditorDto(
        int ExperienceId,
        string Title,
        string ExperienceTypeName,
        EvidenceExperienceType ExperienceType,
        string? TextContent,
        IReadOnlyList<ExperienceEvidenceFileDto> Files);
}

