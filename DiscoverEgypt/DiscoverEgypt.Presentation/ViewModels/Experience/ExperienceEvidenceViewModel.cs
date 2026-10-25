using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed class ExperienceEvidenceViewModel
{
    public required HostExperienceEvidenceEditor Editor { get; init; }
    public string? TextContent { get; set; }
    public string VenueUse { get; set; } = "PublicRoute";
    public bool EvidenceInHostName { get; set; } = true;
    public EvidenceKind FileKind { get; set; }
}
