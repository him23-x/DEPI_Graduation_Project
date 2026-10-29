using DiscoverEgypt.Application.DTOs.Earnings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostEarningsRepository
{
    Task<HostEarningsDto> GetForHostAsync(
        string userId,
        PayoutStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
