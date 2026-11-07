using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Host.Earnings
{
    public sealed class HostEarningsRepository : IHostEarningsRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public HostEarningsRepository(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Payout>> GetForHostAsync(
            string userId,
            PayoutStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var hostPayouts = _dbContext.Payouts
                .AsNoTracking()
                .Where(payout => payout.Booking.ProviderUserId == userId
                    && payout.Booking.BookingType == BookingType.Experience
                    && payout.Booking.Experience != null
                    && payout.Booking.Experience.HostProfile.UserId == userId);

            var filteredPayouts = status.HasValue
                ? hostPayouts.Where(payout => payout.Status == status.Value)
                : hostPayouts;
            return await filteredPayouts
                .Include(payout => payout.Booking)
                    .ThenInclude(booking => booking.Experience)
                .OrderByDescending(payout => payout.EligibleAt)
                .ThenByDescending(payout => payout.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetCountAsync(string userId, PayoutStatus? status, CancellationToken cancellationToken = default)
        {
            var hostPayouts = HostPayouts(userId);
            if (status.HasValue) hostPayouts = hostPayouts.Where(payout => payout.Status == status.Value);
            return await hostPayouts.CountAsync(cancellationToken);
        }

        public async Task<IReadOnlyDictionary<PayoutStatus, decimal>> GetTotalsAsync(string userId, CancellationToken cancellationToken = default)
        {
            var totals = await HostPayouts(userId)
                .GroupBy(payout => payout.Status)
                .Select(group => new { Status = group.Key, Amount = group.Sum(payout => payout.Amount) })
                .ToDictionaryAsync(item => item.Status, item => item.Amount, cancellationToken);
            return totals;
        }

        private IQueryable<Payout> HostPayouts(string userId) => _dbContext.Payouts
            .AsNoTracking()
            .Where(payout => payout.Booking.ProviderUserId == userId
                && payout.Booking.BookingType == BookingType.Experience
                && payout.Booking.Experience != null
                && payout.Booking.Experience.HostProfile.UserId == userId);
    }
}

