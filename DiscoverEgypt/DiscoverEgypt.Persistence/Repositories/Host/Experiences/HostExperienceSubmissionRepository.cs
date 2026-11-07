using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Persistence.Repositories.Host.Experiences
{
    public sealed class HostExperienceSubmissionRepository : IHostExperienceSubmissionRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public HostExperienceSubmissionRepository(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Experience?> GetReviewAsync(string userId, int experienceId, CancellationToken cancellationToken = default)
        {
            return GetEditableExperience(userId, experienceId, tracked: false).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<ExperienceSubmissionOutcome> SubmitAsync(string userId, int experienceId, DeclarationType declarationType, string declarationText, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var experience = await GetEditableExperience(userId, experienceId, tracked: true).SingleOrDefaultAsync(cancellationToken);
            if (experience is null) return ExperienceSubmissionOutcome.ExperienceUnavailable;

            var now = DateTime.UtcNow;
            _dbContext.ExperienceDeclarations.Add(new ExperienceDeclaration
            {
                ExperienceId = experience.Id,
                HostUserId = userId,
                DeclarationType = declarationType,
                DeclarationText = declarationText,
                AcceptedAt = now
            });
            experience.Status = ExperienceStatus.PendingApproval;
            experience.RejectionReason = null;
            experience.UpdatedAt = now;
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return ExperienceSubmissionOutcome.Success;
        }

        private IQueryable<Experience> GetEditableExperience(string userId, int experienceId, bool tracked)
        {
            IQueryable<Experience> query = _dbContext.Experiences;
            if (!tracked) query = query.AsNoTracking();
            return query
                .Where(item => item.Id == experienceId && item.HostProfile.UserId == userId
                    && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected))
                .Include(item => item.ExperienceType)
                .Include(item => item.CulturalArea)
                .Include(item => item.Media)
                .Include(item => item.Evidence.Where(evidence => evidence.DeletedAt == null))
                .Include(item => item.ExperienceInterests)
                .Include(item => item.ExperienceLanguages);
        }
    }
}

