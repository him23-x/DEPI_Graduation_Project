using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IHostExperienceSubmissionRepository
    {
        Task<Experience?> GetReviewAsync(string userId, int experienceId, CancellationToken cancellationToken = default);
        Task<ExperienceSubmissionOutcome> SubmitAsync(string userId, int experienceId, DeclarationType declarationType, string declarationText, CancellationToken cancellationToken = default);
    }
}

