using System.Data;
using DiscoverEgypt.Application.Contracts.Notifications;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.Bookings.Options;
using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Notifications;
using DiscoverEgypt.Domain.Enums.Payments;
using DiscoverEgypt.Domain.Enums.Verification;

namespace DiscoverEgypt.Persistence.Services.Bookings;

public sealed class BookingLifecycleService(
    DiscoverEgyptDbContext dbContext,
    INotificationService notificationService,
    IOptions<BookingLifecycleOptions> lifecycleOptions) : IBookingLifecycleService
{
    private readonly BookingLifecycleOptions _options = lifecycleOptions.Value;

    public async Task CancelForNoResponseAsync(DateTime now, CancellationToken cancellationToken = default)
    {
        var cutoff = now - _options.ProviderResponseTimeout;
        var dueBookingIds = await dbContext.Bookings.AsNoTracking()
            .Where(booking => booking.Status == BookingStatus.Pending
                && booking.Payment.Status == PaymentStatus.Paid
                && booking.Payment.PaidAt.HasValue
                && booking.Payment.PaidAt.Value < cutoff)
            .Select(booking => booking.Id)
            .ToListAsync(cancellationToken);

        foreach (var bookingId in dueBookingIds)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var booking = await dbContext.Bookings
                .Include(item => item.Payment)
                .Include(item => item.Experience)
                .Include(item => item.GuideProfile)
                .SingleOrDefaultAsync(item => item.Id == bookingId, cancellationToken);
            if (booking is null
                || booking.Status != BookingStatus.Pending
                || booking.Payment.Status != PaymentStatus.Paid
                || !booking.Payment.PaidAt.HasValue
                || booking.Payment.PaidAt.Value >= cutoff)
            {
                await transaction.RollbackAsync(cancellationToken);
                continue;
            }

            await CancelPaidBookingAsync(
                booking,
                now,
                "The provider did not respond before the response deadline. The service amount was refunded; the platform fee is not refundable.",
                cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }

    public async Task CancelUnpaidBookingsAsync(DateTime now, CancellationToken cancellationToken = default)
    {
        var cutoff = now - _options.UnpaidBookingTimeout;
        var dueBookingIds = await dbContext.Bookings.AsNoTracking()
            .Where(booking => booking.Status == BookingStatus.Pending
                && (booking.Payment.Status == PaymentStatus.Pending || booking.Payment.Status == PaymentStatus.Failed)
                && booking.Payment.CreatedAt <= cutoff)
            .Select(booking => booking.Id)
            .ToListAsync(cancellationToken);

        foreach (var bookingId in dueBookingIds)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var booking = await dbContext.Bookings
                .Include(item => item.Payment)
                .Include(item => item.Experience)
                .Include(item => item.GuideProfile)
                .SingleOrDefaultAsync(item => item.Id == bookingId, cancellationToken);
            if (booking is null
                || booking.Status != BookingStatus.Pending
                || (booking.Payment.Status != PaymentStatus.Pending && booking.Payment.Status != PaymentStatus.Failed)
                || booking.Payment.CreatedAt > cutoff)
            {
                await transaction.RollbackAsync(cancellationToken);
                continue;
            }

            booking.Status = BookingStatus.Cancelled;
            var title = GetBookingTitle(booking);
            var message = $"The unpaid booking for {title} was closed after the payment window expired.";
            await notificationService.CreateAsync(booking.TouristUserId, NotificationType.BookingStatusUpdate, message, booking.Id, cancellationToken);
            await notificationService.CreateAsync(booking.ProviderUserId, NotificationType.BookingStatusUpdate, message, booking.Id, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }

    public async Task AutoCompleteAsync(DateTime now, CancellationToken cancellationToken = default)
    {
        var completionCutoff = now.AddHours(-24);
        var dueBookingIds = await dbContext.Bookings.AsNoTracking()
            .Where(booking => booking.Status == BookingStatus.Confirmed && booking.ScheduledEnd <= completionCutoff)
            .Select(booking => booking.Id)
            .ToListAsync(cancellationToken);

        foreach (var bookingId in dueBookingIds)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var booking = await dbContext.Bookings
                .Include(item => item.Experience)
                .Include(item => item.GuideProfile)
                .Include(item => item.Disputes)
                .SingleOrDefaultAsync(item => item.Id == bookingId, cancellationToken);
            if (booking is null
                || booking.Status != BookingStatus.Confirmed
                || booking.ScheduledEnd > completionCutoff
                || booking.Disputes.Any(dispute => dispute.Status is DisputeStatus.Open or DisputeStatus.UnderReview))
            {
                await transaction.RollbackAsync(cancellationToken);
                continue;
            }

            booking.Status = BookingStatus.Completed;
            var message = $"Your booking for {GetBookingTitle(booking)} was marked complete after the complaint window ended.";
            await notificationService.CreateAsync(booking.TouristUserId, NotificationType.BookingStatusUpdate, message, booking.Id, cancellationToken);
            await notificationService.CreateAsync(booking.ProviderUserId, NotificationType.BookingStatusUpdate, message, booking.Id, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }

    public async Task ReleaseEligiblePayoutsAsync(DateTime now, CancellationToken cancellationToken = default)
    {
        var payoutIds = await dbContext.Payouts.AsNoTracking()
            .Where(payout => payout.Status == PayoutStatus.Pending && payout.EligibleAt <= now)
            .Select(payout => payout.Id)
            .ToListAsync(cancellationToken);

        foreach (var payoutId in payoutIds)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var payout = await dbContext.Payouts
                .Include(item => item.Booking)
                    .ThenInclude(booking => booking.Disputes)
                .Include(item => item.Booking.Experience)
                .Include(item => item.Booking.GuideProfile)
                .SingleOrDefaultAsync(item => item.Id == payoutId, cancellationToken);
            if (payout is null
                || payout.Status != PayoutStatus.Pending
                || payout.EligibleAt > now
                || payout.Booking.Disputes.Any(dispute => dispute.Status is DisputeStatus.Open or DisputeStatus.UnderReview))
            {
                await transaction.RollbackAsync(cancellationToken);
                continue;
            }

            payout.Status = PayoutStatus.Paid;
            payout.PaidAt = now;
            await notificationService.CreateAsync(
                payout.Booking.ProviderUserId,
                NotificationType.PaymentUpdate,
                $"The payout for {GetBookingTitle(payout.Booking)} has been released.",
                payout.BookingId,
                cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }

    public async Task ExpireGuideLicensesAsync(DateTime now, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(now);
        var verificationRequestIds = await dbContext.VerificationRequests.AsNoTracking()
            .Where(request => request.Status == VerificationStatus.Approved && request.LicenseExpiryDate < today)
            .Select(request => request.Id)
            .ToListAsync(cancellationToken);

        foreach (var verificationRequestId in verificationRequestIds)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var request = await dbContext.VerificationRequests
                .Include(item => item.GuideProfile)
                .SingleOrDefaultAsync(item => item.Id == verificationRequestId, cancellationToken);
            if (request is null
                || request.Status != VerificationStatus.Approved
                || request.LicenseExpiryDate >= today)
            {
                await transaction.RollbackAsync(cancellationToken);
                continue;
            }

            request.Status = VerificationStatus.Expired;
            await notificationService.CreateAsync(
                request.GuideProfile.UserId,
                NotificationType.VerificationUpdate,
                "Your guide license has expired. Submit updated verification documents to accept new bookings.",
                cancellationToken: cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            await CancelPendingGuideBookingsAsync(request.GuideProfileId, now, cancellationToken);
        }
    }

    private async Task CancelPendingGuideBookingsAsync(
        int guideProfileId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var bookingIds = await dbContext.Bookings.AsNoTracking()
            .Where(booking => booking.GuideProfileId == guideProfileId && booking.Status == BookingStatus.Pending)
            .Select(booking => booking.Id)
            .ToListAsync(cancellationToken);

        foreach (var bookingId in bookingIds)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var booking = await dbContext.Bookings
                .Include(item => item.Payment)
                .Include(item => item.Experience)
                .Include(item => item.GuideProfile)
                .SingleOrDefaultAsync(item => item.Id == bookingId, cancellationToken);
            if (booking is null || booking.Status != BookingStatus.Pending)
            {
                await transaction.RollbackAsync(cancellationToken);
                continue;
            }

            if (booking.Payment.Status == PaymentStatus.Paid)
            {
                await CancelPaidBookingAsync(
                    booking,
                    now,
                    "The guide license expired before this request was accepted. The service amount was refunded; the platform fee is not refundable.",
                    cancellationToken);
            }
            else
            {
                booking.Status = BookingStatus.Cancelled;
                var message = $"The pending booking for {GetBookingTitle(booking)} was closed because the guide license expired.";
                await notificationService.CreateAsync(booking.TouristUserId, NotificationType.BookingStatusUpdate, message, booking.Id, cancellationToken);
                await notificationService.CreateAsync(booking.ProviderUserId, NotificationType.BookingStatusUpdate, message, booking.Id, cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
    }

    private async Task CancelPaidBookingAsync(
        Booking booking,
        DateTime now,
        string reason,
        CancellationToken cancellationToken)
    {
        booking.Status = BookingStatus.Cancelled;
        booking.Payment.Status = PaymentStatus.Refunded;
        if (booking.ServiceAmount > 0)
        {
            dbContext.Refunds.Add(new Refund
            {
                BookingId = booking.Id,
                Amount = booking.ServiceAmount,
                Reason = reason,
                CreatedAt = now
            });
        }

        var message = $"The booking for {GetBookingTitle(booking)} was cancelled. The service amount was refunded; the platform fee is not refundable.";
        await notificationService.CreateAsync(booking.TouristUserId, NotificationType.BookingStatusUpdate, message, booking.Id, cancellationToken);
        await notificationService.CreateAsync(booking.ProviderUserId, NotificationType.BookingStatusUpdate, message, booking.Id, cancellationToken);
    }

    private static string GetBookingTitle(Booking booking) =>
        booking.Experience?.Title ?? booking.Destination ?? "your booking";
}
