using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IExperienceStatusRepository
    {
        Task<ExperienceStatus?> GetStatusAsync(
            string userId,
            int experienceId,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateStatusAsync(
            string userId,
            int experienceId,
            ExperienceStatus expectedStatus,
            ExperienceStatus newStatus,
            string? rejectionReason,
            DateTime updatedAt,
            CancellationToken cancellationToken = default);
    }
}

