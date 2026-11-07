using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IHostDashboardRepository
    {
        Task<IReadOnlyList<Experience>> GetExperiencesAsync(string userId, CancellationToken cancellationToken = default);
        Task<int> GetPendingRequestCountAsync(string userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Booking>> GetPendingRequestsAsync(string userId, int limit, CancellationToken cancellationToken = default);
        Task<int> GetConfirmedBookingCountAsync(string userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Booking>> GetUpcomingConfirmedBookingsAsync(string userId, DateTime from, int limit, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<PayoutStatus, decimal>> GetPayoutTotalsAsync(string userId, CancellationToken cancellationToken = default);
        Task<int> GetReviewCountAsync(string userId, CancellationToken cancellationToken = default);
        Task<decimal> GetAverageRatingAsync(string userId, CancellationToken cancellationToken = default);
        Task<int> GetUnreadNotificationCountAsync(string userId, CancellationToken cancellationToken = default);
    }
}

