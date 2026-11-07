using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Domain.Enums.Experiences;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Host.Experiences
{
    public sealed class HostExperienceStatusRepository
        : IExperienceStatusRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public HostExperienceStatusRepository(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ExperienceStatus?> GetStatusAsync(
            string userId,
            int experienceId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            return _dbContext.Experiences
                .Where(item => item.Id == experienceId && item.HostProfile.UserId == userId)
                .Select(item => (ExperienceStatus?)item.Status)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> UpdateStatusAsync(
            string userId,
            int experienceId,
            ExperienceStatus expectedStatus,
            ExperienceStatus newStatus,
            string? rejectionReason,
            DateTime updatedAt,
            CancellationToken cancellationToken = default)
        {
            var updated = await _dbContext.Experiences
                .Where(item => item.Id == experienceId
                    && item.HostProfile.UserId == userId
                    && item.Status == expectedStatus)
                .ExecuteUpdateAsync(update => update
                    .SetProperty(item => item.Status, newStatus)
                    .SetProperty(item => item.RejectionReason, rejectionReason)
                    .SetProperty(item => item.UpdatedAt, updatedAt), cancellationToken);
            return updated == 1;
        }
    }
}

