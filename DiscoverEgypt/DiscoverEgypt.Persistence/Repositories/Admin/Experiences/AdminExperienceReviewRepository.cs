using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.AdminExperiences;
using DiscoverEgypt.Application.Responses.AdminExperiences;
using DiscoverEgypt.Domain.Enums.Experiences;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Admin.Experiences
{
    public sealed class AdminExperienceReviewRepository : IAdminExperienceReviewRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public AdminExperienceReviewRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public async Task<IReadOnlyList<AdminExperienceQueueItemResponse>> GetForManagementAsync(
            CancellationToken cancellationToken = default)
        {
            var items = await _dbContext.Experiences.AsNoTracking()
                .Where(experience => experience.Status == ExperienceStatus.PendingApproval
                    || experience.Status == ExperienceStatus.Published
                    || experience.Status == ExperienceStatus.Unpublished)
                .OrderByDescending(experience => experience.Status == ExperienceStatus.PendingApproval)
                .ThenBy(experience => experience.UpdatedAt)
                .ThenBy(experience => experience.Id)
                .Select(experience => new AdminExperienceQueueItemResponse(
                    experience.Id,
                    experience.Title,
                    experience.ExperienceType.Name,
                    experience.CulturalArea.Name,
                    _dbContext.ApplicationUsers.Where(user => user.Id == experience.HostProfile.UserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? "Discover Egypt host",
                    experience.UpdatedAt,
                    experience.Media.Count(media => media.MediaType == MediaType.Image),
                    experience.Media.Count(media => media.MediaType == MediaType.Video),
                    experience.Status))
                .ToListAsync(cancellationToken);

            return items.Select(item => item with
            {
                SubmittedAt = DateTime.SpecifyKind(item.SubmittedAt, DateTimeKind.Utc)
            }).ToArray();
        }

        public async Task<AdminExperienceDetailsResponse?> GetDetailsAsync(
            int experienceId,
            CancellationToken cancellationToken = default)
        {
            var details = await _dbContext.Experiences.AsNoTracking()
                .Where(experience => experience.Id == experienceId
                    && (experience.Status == ExperienceStatus.PendingApproval
                        || experience.Status == ExperienceStatus.Published
                        || experience.Status == ExperienceStatus.Unpublished))
                .Select(experience => new AdminExperienceDetailsResponse(
                    experience.Id,
                    experience.Title,
                    experience.ExperienceType.Name,
                    experience.CulturalArea.Name,
                    _dbContext.ApplicationUsers.Where(user => user.Id == experience.HostProfile.UserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? "Discover Egypt host",
                    experience.Description,
                    experience.CulturalStory,
                    experience.PricePerPerson,
                    experience.DurationMinutes,
                    experience.MaxGroupSize,
                    experience.MeetingPointArea,
                    experience.ExactMeetingPoint,
                    experience.Declarations.OrderByDescending(declaration => declaration.AcceptedAt)
                        .Select(declaration => (DateTime?)declaration.AcceptedAt).FirstOrDefault() ?? experience.UpdatedAt,
                    experience.ExperienceInterests.OrderBy(link => link.Interest.Name)
                        .Select(link => link.Interest.Name).ToList(),
                    experience.ExperienceLanguages.OrderBy(link => link.Language.Name)
                        .Select(link => link.Language.Name).ToList(),
                    experience.Media.Count(media => media.MediaType == MediaType.Image),
                    experience.Media.Count(media => media.MediaType == MediaType.Video),
                    experience.Media.Any(media => media.MediaType == MediaType.Image && media.IsCover),
                    experience.Evidence.Where(evidence => evidence.DeletedAt == null)
                        .OrderBy(evidence => evidence.EvidenceKind).ThenBy(evidence => evidence.UploadedAt)
                        .Select(evidence => new AdminExperienceEvidenceResponse(
                            evidence.Id,
                            evidence.EvidenceKind,
                            evidence.TextContent,
                            evidence.UploadedAt,
                            evidence.StorageKey != null))
                        .ToList(),
                    experience.Declarations.OrderBy(declaration => declaration.AcceptedAt)
                        .Select(declaration => new AdminExperienceDeclarationResponse(
                            declaration.DeclarationType,
                            declaration.DeclarationText,
                            declaration.AcceptedAt))
                        .ToList(),
                    experience.Status,
                    experience.RejectionReason,
                    experience.Media.Count(media => media.MediaType == MediaType.Image && media.IsCover)))
                .SingleOrDefaultAsync(cancellationToken);

            return details is null
                ? null
                : details with
                {
                    SubmittedAt = DateTime.SpecifyKind(details.SubmittedAt, DateTimeKind.Utc),
                    Evidence = details.Evidence.Select(evidence => evidence with
                    {
                        UploadedAt = DateTime.SpecifyKind(evidence.UploadedAt, DateTimeKind.Utc)
                    }).ToArray(),
                    Declarations = details.Declarations.Select(declaration => declaration with
                    {
                        AcceptedAt = DateTime.SpecifyKind(declaration.AcceptedAt, DateTimeKind.Utc)
                    }).ToArray()
                };
        }

        public async Task<ExperienceReviewDecisionOutcome> DecideAsync(
            string adminUserId,
            int experienceId,
            ExperienceReviewDecision decision,
            string? rejectionReason,
            DateTime nowUtc,
            CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);

            var experience = await _dbContext.Experiences
                .SingleOrDefaultAsync(item => item.Id == experienceId, cancellationToken);
            if (experience is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ExperienceReviewDecisionOutcome.ExperienceUnavailable;
            }

            var expectedStatus = decision switch
            {
                ExperienceReviewDecision.Approve or ExperienceReviewDecision.Reject => ExperienceStatus.PendingApproval,
                ExperienceReviewDecision.Unpublish => ExperienceStatus.Published,
                ExperienceReviewDecision.Republish => ExperienceStatus.Unpublished,
                _ => throw new ArgumentOutOfRangeException(nameof(decision), decision, "Unknown review decision.")
            };
            if (experience.Status != expectedStatus)
            {
                await transaction.RollbackAsync(cancellationToken);
                return decision switch
                {
                    ExperienceReviewDecision.Approve or ExperienceReviewDecision.Reject => ExperienceReviewDecisionOutcome.NotPending,
                    ExperienceReviewDecision.Unpublish => ExperienceReviewDecisionOutcome.NotPublished,
                    ExperienceReviewDecision.Republish => ExperienceReviewDecisionOutcome.NotUnpublished,
                    _ => ExperienceReviewDecisionOutcome.NotPending
                };
            }

            experience.Status = decision switch
            {
                ExperienceReviewDecision.Approve => ExperienceStatus.Published,
                ExperienceReviewDecision.Reject => ExperienceStatus.Rejected,
                ExperienceReviewDecision.Unpublish => ExperienceStatus.Unpublished,
                ExperienceReviewDecision.Republish => ExperienceStatus.Published,
                _ => throw new ArgumentOutOfRangeException(nameof(decision), decision, "Unknown review decision.")
            };
            experience.ReviewedByAdminId = adminUserId;
            experience.RejectionReason = decision == ExperienceReviewDecision.Reject
                ? rejectionReason
                : null;
            experience.UpdatedAt = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return ExperienceReviewDecisionOutcome.Success;
        }
    }
}
