using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.Contracts.HostExperiences
{
    public interface IExperienceReadinessService
    {
        IReadOnlyList<string> GetIssues(
            ExperienceSubmissionReviewDto review,
            ExperienceVenueUse? venueUse = null,
            bool evidenceInHostName = true);
    }
}

