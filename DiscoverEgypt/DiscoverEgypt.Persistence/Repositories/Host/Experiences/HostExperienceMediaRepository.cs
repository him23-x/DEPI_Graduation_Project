using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using DiscoverEgypt.Application.Contracts.Storage;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Persistence.Repositories.Host.Experiences
{
    public sealed class HostExperienceMediaRepository : IHostExperienceMediaRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public HostExperienceMediaRepository(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Experience?> GetDraftEditorAsync(string userId, int experienceId, CancellationToken cancellationToken = default)
        {
            var experience = await _dbContext.Experiences
                .AsNoTracking()
                .Where(item => item.Id == experienceId
                    && item.HostProfile.UserId == userId
                    && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected))
                .Include(item => item.Media.OrderBy(media => media.DisplayOrder).ThenBy(media => media.Id))
                .SingleOrDefaultAsync(cancellationToken);
            return experience;
        }

        public async Task<StoredExperienceMedia?> GetOwnedMediaAsync(string userId, int mediaId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ExperienceMedia
                .AsNoTracking()
                .Where(media => media.Id == mediaId && media.Experience.HostProfile.UserId == userId)
                .Select(media => new StoredExperienceMedia(media.StorageKey, media.MediaType))
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<ExperienceMediaMutationOutcome> ExecuteMutationAsync(
            string userId,
            int experienceId,
            Func<HostExperienceMediaMutationContext, CancellationToken, Task<ExperienceMediaMutationOutcome>> action,
            CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var experience = await _dbContext.Experiences
                .Include(item => item.Media)
                .SingleOrDefaultAsync(item => item.Id == experienceId
                    && item.HostProfile.UserId == userId, cancellationToken);
            if (experience is null)
            {
                return ExperienceMediaMutationOutcome.ExperienceUnavailable;
            }

            var context = new HostExperienceMediaMutationContext
            {
                Experience = experience,
                SaveChangesAsync = token => _dbContext.SaveChangesAsync(token),
                RemoveMedia = media => _dbContext.ExperienceMedia.Remove(media)
            };
            var result = await action(context, cancellationToken);
            if (result != ExperienceMediaMutationOutcome.Success)
            {
                return result;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
    }
}

