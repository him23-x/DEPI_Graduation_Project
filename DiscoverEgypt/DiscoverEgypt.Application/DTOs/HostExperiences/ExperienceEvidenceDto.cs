
using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record ExperienceEvidenceDto
{
    public required HostExperienceEvidenceEditorDto Editor { get; init; }
    public string? TextContent { get; init; }
    public ExperienceVenueUse VenueUse { get; init; } = ExperienceVenueUse.PublicRoute;
    public bool EvidenceInHostName { get; init; } = true;
    public EvidenceKind FileKind { get; init; }
}

