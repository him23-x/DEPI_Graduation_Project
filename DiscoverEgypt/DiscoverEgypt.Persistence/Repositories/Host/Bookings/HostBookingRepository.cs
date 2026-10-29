using System.Data;
using System.Data.Common;
using DiscoverEgypt.Application.Contracts.Notifications;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Options;
using DiscoverEgypt.Application.Features.Bookings.Policies;
using DiscoverEgypt.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

using DiscoverEgypt.Application.Enums.Bookings;

using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Notifications;
using DiscoverEgypt.Domain.Enums.Payments;
using DiscoverEgypt.Domain.Enums.Sanctions;

namespace DiscoverEgypt.Persistence.Repositories.Host.Bookings;

public sealed class HostBookingRepository(
    DiscoverEgyptDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    INotificationService notificationService,
    IOptions<BookingLifecycleOptions> lifecycleOptions) : IHostBookingRepository
{
    public async Task<IReadOnlyList<HostBookingListItemDto>> GetForHostAsync(
        string userId,
        HostBookingTab tab,
        CancellationToken cancellationToken = default)
    {
        var status = ToStatus(tab);
        var bookings = await HostBookings(userId)
            .AsNoTracking()
            .Include(booking => booking.Payment)
            .Include(booking => booking.Experience)
            .Where(booking => booking.Payment.Status == PaymentStatus.Paid
                || (tab == HostBookingTab.RejectedCancelled
                    && (booking.Status == BookingStatus.Rejected || booking.Status == BookingStatus.Cancelled)
                    && booking.Payment.Status == PaymentStatus.Refunded
                    && booking.Refunds.Any()))
            .Where(booking => (tab == HostBookingTab.RejectedCancelled
                ? booking.Status == BookingStatus.Rejected || booking.Status == BookingStatus.Cancelled
                : booking.Status == status))
            .OrderBy(booking => booking.ScheduledStart)
            .ThenBy(booking => booking.Id)
            .ToListAsync(cancellationToken);

        var names = await GetTouristNamesAsync(bookings.Select(booking => booking.TouristUserId), cancellationToken);
        return bookings.Select(booking => new HostBookingListItemDto(
            booking.Id,
            names.GetValueOrDefault(booking.TouristUserId) ?? "Discover Egypt guest",
            booking.Experience!.Title,
            booking.ScheduledStart,
            booking.ScheduledEnd,
            booking.NumberOfGuests!.Value,
            booking.ServiceAmount,
            booking.Status,
            HostBookingPolicy.GetResponseDeadline(booking.Payment.PaidAt, lifecycleOptions.Value.ProviderResponseTimeout))).ToArray();
    }

    public async Task<HostBookingDetailDto?> GetDetailsAsync(
        string userId,
        int bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await HostBookings(userId)
            .AsNoTracking()
            .Include(item => item.Payment)
            .Include(item => item.Experience)
            .Include(item => item.Payout)
            .Include(item => item.Disputes)
            .Where(item => item.Payment.Status == PaymentStatus.Paid
                || ((item.Status == BookingStatus.Rejected || item.Status == BookingStatus.Cancelled)
                    && item.Payment.Status == PaymentStatus.Refunded
                    && item.Refunds.Any()))
            .SingleOrDefaultAsync(item => item.Id == bookingId, cancellationToken);
        if (booking is null) return null;

        var tourist = await userManager.FindByIdAsync(booking.TouristUserId);
        var now = DateTime.UtcNow;
        var isSuspended = await IsProviderSuspendedAsync(userId, now, cancellationToken);
        var hasOverlap = booking.Status == BookingStatus.Pending
            && await HasConfirmedOverlapAsync(userId, booking.ScheduledStart, booking.ScheduledEnd, booking.Id, cancellationToken);
        var hasOpenDispute = booking.Disputes.Any(dispute => dispute.Status is DisputeStatus.Open or DisputeStatus.UnderReview);
        var acceptFailure = HostBookingPolicy.GetAcceptFailure(
            booking.Status, booking.Payment.Status, booking.Payment.PaidAt, isSuspended,
            hasOverlap, booking.Payout is not null, now, lifecycleOptions.Value.ProviderResponseTimeout);

        return new HostBookingDetailDto(
            booking.Id,
            tourist?.FullName ?? tourist?.UserName ?? "Discover Egypt guest",
            tourist?.Email ?? string.Empty,
            booking.Experience!.Title,
            booking.ScheduledStart,
            booking.ScheduledEnd,
            booking.NumberOfGuests!.Value,
            booking.ServiceAmount,
            booking.PlatformFeeAmount,
            booking.TotalAmount,
            booking.Status,
            booking.Payment.Status,
            booking.Payment.PaidAt,
            HostBookingPolicy.GetResponseDeadline(booking.Payment.PaidAt, lifecycleOptions.Value.ProviderResponseTimeout),
            booking.Experience.MeetingPointArea,
            booking.Experience.ExactMeetingPoint,
            booking.Payout?.Amount,
            booking.Payout?.EligibleAt,
            booking.Payout?.Status,
            hasOpenDispute,
            acceptFailure is null,
            HostBookingPolicy.GetRejectFailure(booking.Status, booking.Payment.Status, booking.ServiceAmount) is null,
            HostBookingPolicy.GetCompleteFailure(booking.Status, booking.ScheduledEnd, hasOpenDispute, now) is null);
    }

    public async Task<HostBookingActionOutcome> AcceptAsync(
        string userId,
        int bookingId,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        var hostProfileId = await GetHostProfileIdAsync(userId, cancellationToken);
        if (!hostProfileId.HasValue) return HostBookingActionOutcome.BookingUnavailable;

        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        if (!await AcquireHostBookingLockAsync(hostProfileId.Value, cancellationToken))
            return HostBookingActionOutcome.ConcurrencyUnavailable;

        var booking = await HostBookings(userId)
            .Include(item => item.Payment)
            .Include(item => item.Experience)
            .Include(item => item.Payout)
            .SingleOrDefaultAsync(item => item.Id == bookingId, cancellationToken);
        if (booking is null) return HostBookingActionOutcome.BookingUnavailable;

        var isSuspended = await IsProviderSuspendedAsync(userId, now, cancellationToken);
        var hasOverlap = await HasConfirmedOverlapAsync(userId, booking.ScheduledStart, booking.ScheduledEnd, booking.Id, cancellationToken);
        var failure = HostBookingPolicy.GetAcceptFailure(
            booking.Status, booking.Payment.Status, booking.Payment.PaidAt, isSuspended,
            hasOverlap, booking.Payout is not null, now, lifecycleOptions.Value.ProviderResponseTimeout);
        if (failure.HasValue) return failure.Value;

        booking.Status = BookingStatus.Confirmed;
        dbContext.Payouts.Add(new Payout
        {
            BookingId = booking.Id,
            Amount = booking.ServiceAmount,
            EligibleAt = booking.ScheduledEnd.AddHours(24),
            Status = PayoutStatus.Pending
        });
        await notificationService.CreateAsync(
            booking.TouristUserId,
            NotificationType.BookingConfirmation,
            $"Your booking for {booking.Experience!.Title} has been confirmed.",
            booking.Id,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return HostBookingActionOutcome.Success;
    }

    public async Task<HostBookingActionOutcome> RejectAsync(
        string userId,
        int bookingId,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        var hostProfileId = await GetHostProfileIdAsync(userId, cancellationToken);
        if (!hostProfileId.HasValue) return HostBookingActionOutcome.BookingUnavailable;
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        if (!await AcquireHostBookingLockAsync(hostProfileId.Value, cancellationToken))
            return HostBookingActionOutcome.ConcurrencyUnavailable;
        var booking = await HostBookings(userId)
            .Include(item => item.Payment)
            .Include(item => item.Experience)
            .Include(item => item.Payout)
            .SingleOrDefaultAsync(item => item.Id == bookingId, cancellationToken);
        if (booking is null) return HostBookingActionOutcome.BookingUnavailable;

        var failure = HostBookingPolicy.GetRejectFailure(booking.Status, booking.Payment.Status, booking.ServiceAmount);
        if (failure.HasValue) return failure.Value;
        if (booking.Payout is not null) return HostBookingActionOutcome.PayoutAlreadyExists;

        booking.Status = BookingStatus.Rejected;
        booking.Payment.Status = PaymentStatus.Refunded;
        dbContext.Refunds.Add(new Refund
        {
            BookingId = booking.Id,
            Amount = booking.ServiceAmount,
            Reason = "Host rejected the booking. The service amount is refunded; the platform fee is not refundable.",
            CreatedAt = now
        });
        await notificationService.CreateAsync(
            booking.TouristUserId,
            NotificationType.BookingStatusUpdate,
            $"Your booking for {booking.Experience!.Title} was declined. The service amount will be refunded; the platform fee is not refundable.",
            booking.Id,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return HostBookingActionOutcome.Success;
    }

    public async Task<HostBookingActionOutcome> CompleteAsync(
        string userId,
        int bookingId,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        var hostProfileId = await GetHostProfileIdAsync(userId, cancellationToken);
        if (!hostProfileId.HasValue) return HostBookingActionOutcome.BookingUnavailable;
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        if (!await AcquireHostBookingLockAsync(hostProfileId.Value, cancellationToken))
            return HostBookingActionOutcome.ConcurrencyUnavailable;
        var booking = await HostBookings(userId)
            .Include(item => item.Disputes)
            .SingleOrDefaultAsync(item => item.Id == bookingId, cancellationToken);
        if (booking is null) return HostBookingActionOutcome.BookingUnavailable;

        var hasOpenDispute = booking.Disputes.Any(dispute => dispute.Status is DisputeStatus.Open or DisputeStatus.UnderReview);
        var failure = HostBookingPolicy.GetCompleteFailure(booking.Status, booking.ScheduledEnd, hasOpenDispute, now);
        if (failure.HasValue) return failure.Value;

        booking.Status = BookingStatus.Completed;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return HostBookingActionOutcome.Success;
    }

    private IQueryable<Booking> HostBookings(string userId) => dbContext.Bookings
        .Where(booking => booking.ProviderUserId == userId
            && booking.BookingType == BookingType.Experience
            && booking.Experience != null
            && booking.Experience.HostProfile.UserId == userId);

    private async Task<bool> IsProviderSuspendedAsync(string userId, DateTime now, CancellationToken cancellationToken) =>
        await dbContext.Sanctions.AnyAsync(sanction => sanction.UserId == userId
            && sanction.SanctionType == SanctionType.ProviderSuspension
            && sanction.StartsAt <= now
            && sanction.EndsAt >= now, cancellationToken);

    private Task<int?> GetHostProfileIdAsync(string userId, CancellationToken cancellationToken) => dbContext.HostProfiles
        .Where(profile => profile.UserId == userId)
        .Select(profile => (int?)profile.Id)
        .SingleOrDefaultAsync(cancellationToken);

    private async Task<bool> HasConfirmedOverlapAsync(
        string userId,
        DateTime scheduledStart,
        DateTime scheduledEnd,
        int exceptBookingId,
        CancellationToken cancellationToken) =>
        await dbContext.Bookings.AnyAsync(booking => booking.Id != exceptBookingId
            && booking.ProviderUserId == userId
            && booking.BookingType == BookingType.Experience
            && booking.Status == BookingStatus.Confirmed
            && booking.ScheduledStart < scheduledEnd
            && booking.ScheduledEnd > scheduledStart, cancellationToken);

    private async Task<bool> AcquireHostBookingLockAsync(int hostProfileId, CancellationToken cancellationToken)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();
        await using (command)
        {
            command.Transaction = dbContext.Database.CurrentTransaction!.GetDbTransaction();
            command.CommandText = "DECLARE @lockResult int; EXEC @lockResult = sys.sp_getapplock @Resource = @resource, @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 10000; SELECT @lockResult;";
            var resource = command.CreateParameter();
            resource.ParameterName = "@resource";
            resource.Value = $"DiscoverEgypt:HostExperienceBookingAccept:{hostProfileId}";
            command.Parameters.Add(resource);
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result is not null && Convert.ToInt32(result) >= 0;
        }
    }

    private async Task<Dictionary<string, string>> GetTouristNamesAsync(
        IEnumerable<string> touristIds,
        CancellationToken cancellationToken)
    {
        var names = new Dictionary<string, string>();
        foreach (var touristId in touristIds.Distinct())
        {
            var tourist = await userManager.FindByIdAsync(touristId);
            names[touristId] = tourist?.FullName ?? tourist?.UserName ?? "Discover Egypt guest";
        }
        return names;
    }

    private static BookingStatus ToStatus(HostBookingTab tab) => tab switch
    {
        HostBookingTab.Pending => BookingStatus.Pending,
        HostBookingTab.Confirmed => BookingStatus.Confirmed,
        HostBookingTab.Completed => BookingStatus.Completed,
        _ => BookingStatus.Rejected
    };
}
