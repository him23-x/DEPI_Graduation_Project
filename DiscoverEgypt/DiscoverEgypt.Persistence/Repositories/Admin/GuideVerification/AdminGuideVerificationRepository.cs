using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Enums.GuideVerification;
using DiscoverEgypt.Application.Responses.GuideVerification;
using DiscoverEgypt.Domain.Enums.Verification;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Admin.GuideVerification
{
    public sealed class AdminGuideVerificationRepository : IAdminGuideVerificationRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public AdminGuideVerificationRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public async Task<IReadOnlyList<AdminGuideVerificationQueueItemResponse>> GetPendingAsync(
            CancellationToken cancellationToken = default)
        {
            var items = await _dbContext.VerificationRequests.AsNoTracking()
                .Where(request => request.Status == VerificationStatus.Pending)
                .OrderBy(request => request.SubmittedAt)
                .ThenBy(request => request.Id)
                .Select(request => new AdminGuideVerificationQueueItemResponse(
                    request.Id,
                    request.GuideProfileId,
                    _dbContext.ApplicationUsers.Where(user => user.Id == request.GuideProfile.UserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? "Discover Egypt guide",
                    request.LicenseExpiryDate,
                    request.SubmittedAt,
                    request.Documents.Count(document => document.DeletedAt == null)))
                .ToListAsync(cancellationToken);

            return items.Select(item => item with
            {
                SubmittedAt = DateTime.SpecifyKind(item.SubmittedAt, DateTimeKind.Utc)
            }).ToArray();
        }

        public async Task<AdminGuideVerificationDetailsResponse?> GetPendingDetailsAsync(
            int verificationRequestId,
            CancellationToken cancellationToken = default)
        {
            var details = await _dbContext.VerificationRequests.AsNoTracking()
                .Where(request => request.Id == verificationRequestId && request.Status == VerificationStatus.Pending)
                .Select(request => new AdminGuideVerificationDetailsResponse(
                    request.Id,
                    request.GuideProfileId,
                    _dbContext.ApplicationUsers.Where(user => user.Id == request.GuideProfile.UserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? "Discover Egypt guide",
                    request.GuideProfile.Bio,
                    request.GuideProfile.HourlyRate,
                    request.LicenseExpiryDate,
                    request.SubmittedAt,
                    request.GuideProfile.GuideLanguages.OrderBy(link => link.Language.Name)
                        .Select(link => link.Language.Name + (link.IsNative ? " (native)" : ""))
                        .ToList(),
                    request.GuideProfile.GuideCulturalAreas.OrderBy(link => link.CulturalArea.Name)
                        .Select(link => link.CulturalArea.Name).ToList(),
                    request.Documents.Where(document => document.DeletedAt == null)
                        .OrderBy(document => document.DocType).ThenBy(document => document.UploadedAt)
                        .Select(document => new AdminGuideVerificationDocumentResponse(
                            document.Id,
                            document.DocType,
                            document.Language == null ? null : document.Language.Name,
                            document.CulturalArea == null ? null : document.CulturalArea.Name,
                            document.UploadedAt))
                        .ToList()))
                .SingleOrDefaultAsync(cancellationToken);

            return details is null
                ? null
                : details with
                {
                    SubmittedAt = DateTime.SpecifyKind(details.SubmittedAt, DateTimeKind.Utc),
                    Documents = details.Documents.Select(document => document with
                    {
                        UploadedAt = DateTime.SpecifyKind(document.UploadedAt, DateTimeKind.Utc)
                    }).ToArray()
                };
        }

        public Task<StoredGuideVerificationDocument?> GetDocumentForReviewAsync(
            int documentId,
            CancellationToken cancellationToken = default) =>
            _dbContext.VerificationDocuments.AsNoTracking()
                .Where(document => document.Id == documentId
                    && document.DeletedAt == null
                    && document.VerificationRequest.Status == VerificationStatus.Pending)
                .Select(document => new StoredGuideVerificationDocument(
                    document.Id,
                    document.DocType,
                    document.StorageKey))
                .SingleOrDefaultAsync(cancellationToken);

        public async Task<GuideVerificationDecisionOutcome> DecideAsync(
            string adminUserId,
            int verificationRequestId,
            GuideVerificationDecision decision,
            string? rejectionReason,
            DateTime reviewedAtUtc,
            CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);

            var request = await _dbContext.VerificationRequests
                .SingleOrDefaultAsync(item => item.Id == verificationRequestId, cancellationToken);
            if (request is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return GuideVerificationDecisionOutcome.RequestUnavailable;
            }

            if (request.Status != VerificationStatus.Pending)
            {
                await transaction.RollbackAsync(cancellationToken);
                return GuideVerificationDecisionOutcome.NotPending;
            }

            request.Status = decision switch
            {
                GuideVerificationDecision.Approve => VerificationStatus.Approved,
                GuideVerificationDecision.Reject => VerificationStatus.Rejected,
                _ => throw new ArgumentOutOfRangeException(nameof(decision), decision, "Unknown guide verification decision.")
            };
            request.ReviewedByAdminId = adminUserId;
            request.ReviewedAt = DateTime.SpecifyKind(reviewedAtUtc, DateTimeKind.Utc);
            request.RejectionReason = decision == GuideVerificationDecision.Reject ? rejectionReason : null;

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return GuideVerificationDecisionOutcome.Success;
        }
    }
}
