using DiscoverEgypt.Application.DTOs.HostExperiences;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed class ExperienceReviewViewModel
{
    public required HostExperienceSubmissionReview Review { get; init; }
    public string DeclarationText { get; init; } = string.Empty;
    public bool Accepted { get; set; }
    public ExperienceVenueUse VenueUse { get; set; } = ExperienceVenueUse.PublicRoute;
    public bool EvidenceInHostName { get; set; } = true;
    public string? Error { get; init; }
}
