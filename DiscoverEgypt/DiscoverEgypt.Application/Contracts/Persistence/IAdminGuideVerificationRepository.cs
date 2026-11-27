using DiscoverEgypt.Application.Enums.GuideVerification;
using DiscoverEgypt.Application.Responses.GuideVerification;
using DiscoverEgypt.Application.Contracts.Storage;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IAdminGuideVerificationRepository
    {
        Task<IReadOnlyList<AdminGuideVerificationQueueItemResponse>> GetPendingAsync(
            CancellationToken cancellationToken = default);

        Task<AdminGuideVerificationDetailsResponse?> GetPendingDetailsAsync(
            int verificationRequestId,
            CancellationToken cancellationToken = default);

        Task<StoredGuideVerificationDocument?> GetDocumentForReviewAsync(
            int documentId,
            CancellationToken cancellationToken = default);

        Task<GuideVerificationDecisionOutcome> DecideAsync(
            string adminUserId,
            int verificationRequestId,
            GuideVerificationDecision decision,
            string? rejectionReason,
            DateTime reviewedAtUtc,
            CancellationToken cancellationToken = default);
    }
}
