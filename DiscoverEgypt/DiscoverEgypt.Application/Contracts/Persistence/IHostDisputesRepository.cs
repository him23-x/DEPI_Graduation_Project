using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Application.Responses.Accounts;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IHostDisputesRepository
    {
        Task<IReadOnlyList<Dispute>> GetForHostAsync(
            string userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<int> GetCountAsync(string userId, CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<string, UserSummaryResponse>> GetTouristsAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default);

        Task<Dispute?> GetDetailsAsync(
            string userId,
            int disputeId,
            CancellationToken cancellationToken = default);
    }
}

