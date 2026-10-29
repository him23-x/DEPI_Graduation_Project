using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Earnings;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Host.Earnings;

public sealed class HostEarningsRepository(DiscoverEgyptDbContext dbContext) : IHostEarningsRepository
{
    public async Task<HostEarningsDto> GetForHostAsync(
        string userId,
        PayoutStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var hostPayouts = dbContext.Payouts
            .AsNoTracking()
            .Where(payout => payout.Booking.ProviderUserId == userId
                && payout.Booking.BookingType == BookingType.Experience
                && payout.Booking.Experience != null
                && payout.Booking.Experience.HostProfile.UserId == userId);

        var pendingAmount = await hostPayouts
            .Where(payout => payout.Status == PayoutStatus.Pending)
            .SumAsync(payout => (decimal?)payout.Amount, cancellationToken) ?? 0m;
        var paidAmount = await hostPayouts
            .Where(payout => payout.Status == PayoutStatus.Paid)
            .SumAsync(payout => (decimal?)payout.Amount, cancellationToken) ?? 0m;
        var cancelledAmount = await hostPayouts
            .Where(payout => payout.Status == PayoutStatus.Cancelled)
            .SumAsync(payout => (decimal?)payout.Amount, cancellationToken) ?? 0m;

        var filteredPayouts = status.HasValue
            ? hostPayouts.Where(payout => payout.Status == status.Value)
            : hostPayouts;
        var totalCount = await filteredPayouts.CountAsync(cancellationToken);
        var payouts = await filteredPayouts
            .OrderByDescending(payout => payout.EligibleAt)
            .ThenByDescending(payout => payout.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(payout => new HostPayoutListItemDto(
                payout.Id,
                payout.BookingId,
                payout.Booking.Experience!.Title,
                payout.Booking.ScheduledStart,
                payout.Amount,
                payout.Status,
                payout.EligibleAt,
                payout.PaidAt))
            .ToListAsync(cancellationToken);

        return new HostEarningsDto(
            payouts,
            pendingAmount,
            paidAmount,
            cancelledAmount,
            totalCount,
            page,
            pageSize);
    }
}
