using DiscoverEgypt.Domain.Entities;

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

        Task<IReadOnlyDictionary<string, ApplicationUser>> GetTouristsAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default);
    }
}

