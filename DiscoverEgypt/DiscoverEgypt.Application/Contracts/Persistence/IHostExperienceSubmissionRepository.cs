using DiscoverEgypt.Application.DTOs.HostExperiences;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostExperienceSubmissionRepository
{
    Task<HostExperienceSubmissionReviewDto?> GetReviewAsync(string userId, int experienceId, CancellationToken cancellationToken = default);
    Task<ExperienceSubmissionOutcome> SubmitAsync(string userId, int experienceId, DeclarationType declarationType, string declarationText, ExperienceVenueUse? venueUse, bool evidenceInHostName, CancellationToken cancellationToken = default);
}
