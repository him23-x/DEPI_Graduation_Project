using DiscoverEgypt.Application.Enums.Disputes;
using DiscoverEgypt.Application.Responses.Disputes;
using DiscoverEgypt.Domain.Enums.Disputes;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IAdminDisputeReviewRepository
    {
        Task<IReadOnlyList<AdminDisputeQueueItemResponse>> GetOpenAsync(
            CancellationToken cancellationToken = default);

        Task<AdminDisputeDetailsResponse?> GetOpenDetailsAsync(
            int disputeId,
            CancellationToken cancellationToken = default);

        Task<AdminDisputeResolutionOutcome> ResolveAsync(
            string adminUserId,
            int disputeId,
            DisputeOutcome outcome,
            string? resolutionNote,
            int? sanctionDays,
            DateTime resolvedAtUtc,
            CancellationToken cancellationToken = default);
    }
}
