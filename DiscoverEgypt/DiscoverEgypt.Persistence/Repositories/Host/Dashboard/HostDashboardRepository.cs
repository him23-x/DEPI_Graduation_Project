using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Dashboard;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Experiences;
using DiscoverEgypt.Domain.Enums.Notifications;
using DiscoverEgypt.Domain.Enums.Payments;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Host.Dashboard;

public sealed class HostDashboardRepository(DiscoverEgyptDbContext dbContext) : IHostDashboardRepository
{
    public async Task<HostDashboardDto> GetForHostAsync(
        string userId,
        DateTime utcNow,
        CancellationToken cancellationToken = default)
    {
        var experiences = dbContext.Experiences
            .AsNoTracking()
            .Where(experience => experience.HostProfile.UserId == userId);
        var experienceCounts = await experiences
            .GroupBy(experience => experience.Status)
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.Status, item => item.Count, cancellationToken);
        var experienceStatuses = Enum.GetValues<ExperienceStatus>()
            .Select(status => new HostDashboardExperienceStatusDto(status, experienceCounts.GetValueOrDefault(status)))
            .ToArray();

        var hostBookings = dbContext.Bookings
            .AsNoTracking()
            .Where(booking => booking.ProviderUserId == userId
                && booking.BookingType == BookingType.Experience
                && booking.Experience != null
                && booking.Experience.HostProfile.UserId == userId);

        var pendingRequests = hostBookings
            .Where(booking => booking.Status == BookingStatus.Pending
                && booking.Payment.Status == PaymentStatus.Paid);
        var pendingRequestCount = await pendingRequests.CountAsync(cancellationToken);
        var nearestPendingRequests = await pendingRequests
            .OrderBy(booking => booking.ScheduledStart)
            .ThenBy(booking => booking.Id)
            .Take(3)
            .Select(booking => new HostDashboardBookingDto(
                booking.Id,
                booking.Experience!.Title,
                booking.ScheduledStart))
            .ToListAsync(cancellationToken);

        var confirmedBookings = hostBookings.Where(booking => booking.Status == BookingStatus.Confirmed);
        var confirmedBookingCount = await confirmedBookings.CountAsync(cancellationToken);
        var upcomingConfirmedBookings = await confirmedBookings
            .Where(booking => booking.ScheduledStart >= utcNow)
            .OrderBy(booking => booking.ScheduledStart)
            .ThenBy(booking => booking.Id)
            .Take(3)
            .Select(booking => new HostDashboardBookingDto(
                booking.Id,
                booking.Experience!.Title,
                booking.ScheduledStart))
            .ToListAsync(cancellationToken);

        var hostPayouts = dbContext.Payouts
            .AsNoTracking()
            .Where(payout => payout.Booking.ProviderUserId == userId
                && payout.Booking.BookingType == BookingType.Experience
                && payout.Booking.Experience != null
                && payout.Booking.Experience.HostProfile.UserId == userId);
        var payoutTotals = await hostPayouts
            .GroupBy(payout => payout.Status)
            .Select(group => new { Status = group.Key, Amount = group.Sum(payout => payout.Amount) })
            .ToDictionaryAsync(item => item.Status, item => item.Amount, cancellationToken);

        var hostReviews = dbContext.Reviews
            .AsNoTracking()
            .Where(review => review.Booking.ProviderUserId == userId
                && review.Booking.BookingType == BookingType.Experience
                && review.Booking.Experience != null
                && review.Booking.Experience.HostProfile.UserId == userId);
        var reviewCount = await hostReviews.CountAsync(cancellationToken);
        var averageRating = await hostReviews
            .AverageAsync(review => (decimal?)review.Rating, cancellationToken) ?? 0m;
        var unreadNotificationCount = await dbContext.Notifications
            .AsNoTracking()
            .CountAsync(notification => notification.UserId == userId && !notification.IsRead, cancellationToken);

        return new HostDashboardDto(
            experienceStatuses,
            pendingRequestCount,
            nearestPendingRequests,
            confirmedBookingCount,
            upcomingConfirmedBookings,
            payoutTotals.GetValueOrDefault(PayoutStatus.Pending),
            payoutTotals.GetValueOrDefault(PayoutStatus.Paid),
            payoutTotals.GetValueOrDefault(PayoutStatus.Cancelled),
            unreadNotificationCount,
            averageRating,
            reviewCount);
    }
}
