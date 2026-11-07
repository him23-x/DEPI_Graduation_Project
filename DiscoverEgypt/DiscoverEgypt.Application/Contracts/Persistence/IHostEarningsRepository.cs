using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IHostEarningsRepository
    {
        Task<IReadOnlyList<Payout>> GetForHostAsync(
            string userId,
            PayoutStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<int> GetCountAsync(string userId, PayoutStatus? status, CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<PayoutStatus, decimal>> GetTotalsAsync(string userId, CancellationToken cancellationToken = default);
    }
}

