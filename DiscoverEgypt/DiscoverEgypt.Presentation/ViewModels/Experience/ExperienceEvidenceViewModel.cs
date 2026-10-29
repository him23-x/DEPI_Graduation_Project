using DiscoverEgypt.Application.DTOs.HostExperiences;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed class ExperienceEvidenceViewModel
{
    public required ExperienceEvidenceEditorViewModel Editor { get; init; }
    public string? TextContent { get; set; }
    public ExperienceVenueUse VenueUse { get; set; } = ExperienceVenueUse.PublicRoute;
    public bool EvidenceInHostName { get; set; } = true;
    public EvidenceKind FileKind { get; set; }
}
