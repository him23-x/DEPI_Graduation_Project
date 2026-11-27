
using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences
{
    public sealed class ExperienceReviewDto
    {
        public required ExperienceSubmissionReviewDto Review { get; set; }
        public string DeclarationText { get; set; } = string.Empty;
        public bool Accepted { get; set; }
        public ExperienceVenueUse VenueUse { get; set; } = ExperienceVenueUse.PublicRoute;
        public bool EvidenceInHostName { get; set; } = true;
        public string? Error { get; set; }
    }
}

