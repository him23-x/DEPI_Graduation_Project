
using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record ExperienceReviewDto
{
    public required HostExperienceSubmissionReviewDto Review { get; init; }
    public string DeclarationText { get; init; } = string.Empty;
    public bool Accepted { get; init; }
    public ExperienceVenueUse VenueUse { get; init; } = ExperienceVenueUse.PublicRoute;
    public bool EvidenceInHostName { get; init; } = true;
    public string? Error { get; init; }
}

