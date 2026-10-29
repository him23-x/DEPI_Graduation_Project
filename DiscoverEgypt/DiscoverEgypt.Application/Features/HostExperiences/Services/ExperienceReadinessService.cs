using DiscoverEgypt.Application.Contracts.HostExperiences;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Validators.DTOs;

namespace DiscoverEgypt.Application.Features.HostExperiences.Services;

public sealed class ExperienceReadinessService(HostExperienceSubmissionReviewValidator validator)
    : IExperienceReadinessService
{
    private const string VenueChoiceRequiredMessage =
        "Confirm whether this experience is a public route, private/home venue, or controlled third-party venue.";

    public IReadOnlyList<string> GetIssues(
        HostExperienceSubmissionReviewDto review,
        ExperienceVenueUse? venueUse = null,
        bool evidenceInHostName = true)
    {
        ArgumentNullException.ThrowIfNull(review);

        var issues = validator.GetIssues(review, venueUse, evidenceInHostName).ToList();
        if (review.ExperienceType == EvidenceExperienceType.LocalTraditions && !venueUse.HasValue)
            issues.Add(VenueChoiceRequiredMessage);

        return issues;
    }
}
