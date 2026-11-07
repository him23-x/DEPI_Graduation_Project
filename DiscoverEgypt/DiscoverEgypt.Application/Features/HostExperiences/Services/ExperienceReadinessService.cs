using DiscoverEgypt.Application.Contracts.HostExperiences;
using DiscoverEgypt.Application.ViewModels.HostExperiences;
using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Validators.ViewModels;

namespace DiscoverEgypt.Application.Features.HostExperiences.Services
{
    public sealed class ExperienceReadinessService
        : IExperienceReadinessService
    {
        private readonly HostExperienceSubmissionReviewValidator _validator;

        public ExperienceReadinessService(HostExperienceSubmissionReviewValidator validator)
        {
            _validator = validator;
        }

        private const string VenueChoiceRequiredMessage =
            "Confirm whether this experience is a public route, private/home venue, or controlled third-party venue.";

        public IReadOnlyList<string> GetIssues(
            ExperienceSubmissionReviewViewModel review,
            ExperienceVenueUse? venueUse = null,
            bool evidenceInHostName = true)
        {
            ArgumentNullException.ThrowIfNull(review);

            var issues = _validator.GetIssues(review, venueUse, evidenceInHostName).ToList();
            if (review.ExperienceType == EvidenceExperienceType.LocalTraditions && !venueUse.HasValue)
                issues.Add(VenueChoiceRequiredMessage);

            return issues;
        }
    }
}

