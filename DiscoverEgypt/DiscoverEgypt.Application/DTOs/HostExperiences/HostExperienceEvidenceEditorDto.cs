
using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceEvidenceEditorDto(
    int ExperienceId,
    string Title,
    string ExperienceTypeName,
    EvidenceExperienceType ExperienceType,
    string? TextContent,
    IReadOnlyList<HostExperienceEvidenceFileDto> Files);

