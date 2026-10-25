using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceEvidenceEditor(
    int ExperienceId,
    string Title,
    string ExperienceTypeName,
    EvidenceExperienceType ExperienceType,
    string? TextContent,
    IReadOnlyList<HostExperienceEvidenceFile> Files);

