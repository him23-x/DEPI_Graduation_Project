using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Experiences;
using DiscoverEgypt.Domain.Enums.Notifications;
using DiscoverEgypt.Domain.Enums.Payments;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Host.Dashboard
{
    public sealed class HostDashboardRepository : IHostDashboardRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public HostDashboardRepository(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Experience>> GetExperiencesAsync(string userId, CancellationToken cancellationToken = default) =>
            await _dbContext.Experiences.AsNoTracking().Where(experience => experience.HostProfile.UserId == userId).ToListAsync(cancellationToken);

        private IQueryable<Booking> HostBookings(string userId) => _dbContext.Bookings
                .AsNoTracking()
                .Where(booking => booking.ProviderUserId == userId
                    && booking.BookingType == BookingType.Experience
                    && booking.Experience != null
                    && booking.Experience.HostProfile.UserId == userId);

        public Task<int> GetPendingRequestCountAsync(string userId, CancellationToken cancellationToken = default) =>
            HostBookings(userId).CountAsync(booking => booking.Status == Domain.Enums.Bookings.BookingStatus.Pending && booking.Payment.Status == PaymentStatus.Paid, cancellationToken);

        public async Task<IReadOnlyList<Booking>> GetPendingRequestsAsync(string userId, int limit, CancellationToken cancellationToken = default) =>
            await HostBookings(userId).Where(booking => booking.Status == Domain.Enums.Bookings.BookingStatus.Pending && booking.Payment.Status == PaymentStatus.Paid)
                .Include(booking => booking.Experience).OrderBy(booking => booking.ScheduledStart).ThenBy(booking => booking.Id).Take(limit).ToListAsync(cancellationToken);

        public Task<int> GetConfirmedBookingCountAsync(string userId, CancellationToken cancellationToken = default) =>
            HostBookings(userId).CountAsync(booking => booking.Status == Domain.Enums.Bookings.BookingStatus.Confirmed, cancellationToken);

        public async Task<IReadOnlyList<Booking>> GetUpcomingConfirmedBookingsAsync(string userId, DateTime from, int limit, CancellationToken cancellationToken = default) =>
            await HostBookings(userId).Where(booking => booking.Status == Domain.Enums.Bookings.BookingStatus.Confirmed && booking.ScheduledStart >= from)
                .Include(booking => booking.Experience).OrderBy(booking => booking.ScheduledStart).ThenBy(booking => booking.Id).Take(limit).ToListAsync(cancellationToken);

        public async Task<IReadOnlyDictionary<PayoutStatus, decimal>> GetPayoutTotalsAsync(string userId, CancellationToken cancellationToken = default) =>
            await _dbContext.Payouts.AsNoTracking()
                .Where(payout => payout.Booking.ProviderUserId == userId && payout.Booking.BookingType == Domain.Enums.Bookings.BookingType.Experience
                    && payout.Booking.Experience != null && payout.Booking.Experience.HostProfile.UserId == userId)
                .GroupBy(payout => payout.Status)
                .Select(group => new { Status = group.Key, Amount = group.Sum(payout => payout.Amount) })
                .ToDictionaryAsync(item => item.Status, item => item.Amount, cancellationToken);

        private IQueryable<Review> HostReviews(string userId) => _dbContext.Reviews
                .AsNoTracking()
                .Where(review => review.Booking.ProviderUserId == userId
                    && review.Booking.BookingType == Domain.Enums.Bookings.BookingType.Experience
                    && review.Booking.Experience != null
                    && review.Booking.Experience.HostProfile.UserId == userId);

        public Task<int> GetReviewCountAsync(string userId, CancellationToken cancellationToken = default) => HostReviews(userId).CountAsync(cancellationToken);
        public async Task<decimal> GetAverageRatingAsync(string userId, CancellationToken cancellationToken = default) => await HostReviews(userId).AverageAsync(review => (decimal?)review.Rating, cancellationToken) ?? 0m;
        public Task<int> GetUnreadNotificationCountAsync(string userId, CancellationToken cancellationToken = default) => _dbContext.Notifications.AsNoTracking().CountAsync(notification => notification.UserId == userId && !notification.IsRead, cancellationToken);
    }
}

