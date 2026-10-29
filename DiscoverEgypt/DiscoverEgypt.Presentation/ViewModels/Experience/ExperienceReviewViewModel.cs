using DiscoverEgypt.Application.DTOs.HostExperiences;

using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed class ExperienceReviewViewModel
{
    public required ExperienceSubmissionReviewViewModel Review { get; init; }
    public string DeclarationText { get; init; } = string.Empty;
    public bool Accepted { get; set; }
    public ExperienceVenueUse VenueUse { get; set; } = ExperienceVenueUse.PublicRoute;
    public bool EvidenceInHostName { get; set; } = true;
    public string? Error { get; init; }
}
