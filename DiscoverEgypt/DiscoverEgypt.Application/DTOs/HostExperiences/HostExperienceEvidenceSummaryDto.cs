
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceEvidenceSummaryDto(EvidenceKind Kind, string? TextContent, int FileCount);
