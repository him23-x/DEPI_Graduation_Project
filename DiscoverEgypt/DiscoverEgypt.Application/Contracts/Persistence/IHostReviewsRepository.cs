using DiscoverEgypt.Application.DTOs.Reviews;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostReviewsRepository
{
    Task<HostReviewsPageDto> GetForHostAsync(
        string userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
