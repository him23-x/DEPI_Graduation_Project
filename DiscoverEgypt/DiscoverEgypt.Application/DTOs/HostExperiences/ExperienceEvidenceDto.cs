using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed class ExperienceEvidenceDto
{
    public required HostExperienceEvidenceEditor Editor { get; init; }
    public string? TextContent { get; set; }
    public ExperienceVenueUse VenueUse { get; set; } = ExperienceVenueUse.PublicRoute;
    public bool EvidenceInHostName { get; set; } = true;
    public EvidenceKind FileKind { get; set; }
}

