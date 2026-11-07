using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using DiscoverEgypt.Application.Contracts.Storage;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Persistence.Repositories.Host.Experiences
{
    public sealed class HostExperienceEvidenceRepository : IHostExperienceEvidenceRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public HostExperienceEvidenceRepository(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Experience?> GetDraftEditorAsync(string userId, int experienceId, CancellationToken cancellationToken = default)
        {
            var experience = await _dbContext.Experiences.AsNoTracking()
                .Where(item => item.Id == experienceId && item.HostProfile.UserId == userId
                    && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected))
                .Include(item => item.ExperienceType)
                .Include(item => item.Evidence.Where(evidence => evidence.DeletedAt == null))
                .SingleOrDefaultAsync(cancellationToken);
            return experience;
        }

        public async Task<ExperienceEvidenceMutationOutcome> ExecuteMutationAsync(
            string userId,
            int experienceId,
            Func<HostExperienceEvidenceMutationContext, CancellationToken, Task<ExperienceEvidenceMutationOutcome>> action,
            CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var experience = await _dbContext.Experiences
                .Include(item => item.Evidence)
                .Include(item => item.ExperienceType)
                .SingleOrDefaultAsync(item => item.Id == experienceId && item.HostProfile.UserId == userId, cancellationToken);
            if (experience is null)
            {
                return ExperienceEvidenceMutationOutcome.ExperienceUnavailable;
            }

            var context = new HostExperienceEvidenceMutationContext
            {
                Experience = experience,
                SaveChangesAsync = token => _dbContext.SaveChangesAsync(token)
            };
            var result = await action(context, cancellationToken);
            if (result != ExperienceEvidenceMutationOutcome.Success)
            {
                return result;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        public async Task<StoredExperienceEvidence?> GetOwnedFileAsync(string userId, int evidenceId, CancellationToken cancellationToken = default)
        {
            var key = await _dbContext.ExperienceEvidence.AsNoTracking()
                .Where(item => item.Id == evidenceId && item.Experience.HostProfile.UserId == userId
                    && item.DeletedAt == null && item.StorageKey != null)
                .Select(item => item.StorageKey)
                .SingleOrDefaultAsync(cancellationToken);
            return key is null ? null : new StoredExperienceEvidence(key);
        }

        public async Task<StoredExperienceEvidence?> GetFileForReviewAsync(int evidenceId, CancellationToken cancellationToken = default)
        {
            var key = await _dbContext.ExperienceEvidence.AsNoTracking()
                .Where(item => item.Id == evidenceId && item.DeletedAt == null && item.StorageKey != null)
                .Select(item => item.StorageKey)
                .SingleOrDefaultAsync(cancellationToken);
            return key is null ? null : new StoredExperienceEvidence(key);
        }

    }
}

