using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceEvidenceSummary(EvidenceKind Kind, string? TextContent, int FileCount);

