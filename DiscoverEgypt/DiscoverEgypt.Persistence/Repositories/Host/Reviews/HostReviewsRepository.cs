using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Host.Reviews
{
    public sealed class HostReviewsRepository : IHostReviewsRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public HostReviewsRepository(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Review>> GetForHostAsync(
            string userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var hostReviews = _dbContext.Reviews
                .AsNoTracking()
                .Where(review => review.Booking.ProviderUserId == userId
                    && review.Booking.BookingType == BookingType.Experience
                    && review.Booking.Experience != null
                    && review.Booking.Experience.HostProfile.UserId == userId);

            return await hostReviews
                .Include(review => review.Booking)
                    .ThenInclude(booking => booking.Experience)
                .OrderByDescending(review => review.CreatedAt)
                .ThenByDescending(review => review.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public Task<int> GetCountAsync(string userId, CancellationToken cancellationToken = default) => HostReviews(userId).CountAsync(cancellationToken);

        public async Task<decimal> GetAverageRatingAsync(string userId, CancellationToken cancellationToken = default) =>
            await HostReviews(userId).AverageAsync(review => (decimal?)review.Rating, cancellationToken) ?? 0m;

        public async Task<IReadOnlyDictionary<string, ApplicationUser>> GetTouristsAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
            await _dbContext.ApplicationUsers.AsNoTracking()
                .Where(user => userIds.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id, cancellationToken);

        private IQueryable<Review> HostReviews(string userId) => _dbContext.Reviews
            .AsNoTracking()
            .Where(review => review.Booking.ProviderUserId == userId
                && review.Booking.BookingType == BookingType.Experience
                && review.Booking.Experience != null
                && review.Booking.Experience.HostProfile.UserId == userId);
    }
}

