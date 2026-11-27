using DiscoverEgypt.Application.Responses.AdminExperiences;
using DiscoverEgypt.Application.Enums.AdminExperiences;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IAdminExperienceReviewRepository
    {
        Task<IReadOnlyList<AdminExperienceQueueItemResponse>> GetForManagementAsync(
            CancellationToken cancellationToken = default);

        Task<AdminExperienceDetailsResponse?> GetDetailsAsync(
            int experienceId,
            CancellationToken cancellationToken = default);

        Task<ExperienceReviewDecisionOutcome> DecideAsync(
            string adminUserId,
            int experienceId,
            ExperienceReviewDecision decision,
            string? rejectionReason,
            DateTime nowUtc,
            CancellationToken cancellationToken = default);
    }
}
