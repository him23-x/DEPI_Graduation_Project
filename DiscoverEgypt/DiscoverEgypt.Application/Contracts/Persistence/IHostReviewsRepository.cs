using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Application.Responses.Accounts;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IHostReviewsRepository
    {
        Task<IReadOnlyList<Review>> GetForHostAsync(
            string userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<int> GetCountAsync(string userId, CancellationToken cancellationToken = default);

        Task<decimal> GetAverageRatingAsync(string userId, CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<string, UserSummaryResponse>> GetTouristsAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default);
    }
}

