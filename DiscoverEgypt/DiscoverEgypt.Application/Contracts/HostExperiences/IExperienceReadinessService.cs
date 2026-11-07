using DiscoverEgypt.Application.ViewModels.HostExperiences;
using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.Contracts.HostExperiences
{
    public interface IExperienceReadinessService
    {
        IReadOnlyList<string> GetIssues(
            ExperienceSubmissionReviewViewModel review,
            ExperienceVenueUse? venueUse = null,
            bool evidenceInHostName = true);
    }
}

