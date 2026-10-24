using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostExperienceSubmissionRepository
{
    Task<HostExperienceSubmissionReview?> GetReviewAsync(string userId, int experienceId, CancellationToken cancellationToken = default);
    Task<ExperienceSubmissionResult> SubmitAsync(string userId, int experienceId, DeclarationType declarationType, string declarationText, ExperienceVenueUse? venueUse, bool evidenceInHostName, CancellationToken cancellationToken = default);
}
