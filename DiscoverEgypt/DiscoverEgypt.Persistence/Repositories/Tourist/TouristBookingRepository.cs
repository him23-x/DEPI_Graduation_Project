using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Bookings;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Payments;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Tourist
{
    public sealed class TouristBookingRepository : ITouristBookingRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public TouristBookingRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public async Task<IReadOnlyList<TouristBookingListItemResponse>> GetForTouristAsync(
            string userId,
            CancellationToken cancellationToken = default) =>
            await _dbContext.Bookings.AsNoTracking()
                .Where(booking => booking.TouristUserId == userId)
                .OrderByDescending(booking => booking.CreatedAt)
                .ThenByDescending(booking => booking.Id)
                .Select(booking => new TouristBookingListItemResponse(
                    booking.Id,
                    booking.BookingType,
                    booking.BookingType == BookingType.Experience
                        ? booking.Experience!.Title
                        : _dbContext.ApplicationUsers.Where(user => user.Id == booking.ProviderUserId)
                            .Select(user => user.FullName).FirstOrDefault() ?? "Local guide",
                    booking.ScheduledStart,
                    booking.ScheduledEnd,
                    booking.NumberOfGuests,
                    booking.GuideHours,
                    booking.Status,
                    booking.Payment.Status,
                    booking.TotalAmount,
                    booking.Refunds.Sum(refund => (decimal?)refund.Amount) ?? 0m,
                    booking.Payout == null ? null : booking.Payout.Status))
                .ToListAsync(cancellationToken);

        public Task<TouristBookingDetailsResponse?> GetDetailsForTouristAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken = default) =>
            _dbContext.Bookings.AsNoTracking()
                .Where(booking => booking.TouristUserId == userId && booking.Id == bookingId)
                .Select(booking => new TouristBookingDetailsResponse(
                    booking.Id,
                    booking.BookingType,
                    booking.BookingType == BookingType.Experience
                        ? booking.Experience!.Title
                        : _dbContext.ApplicationUsers.Where(user => user.Id == booking.ProviderUserId)
                            .Select(user => user.FullName).FirstOrDefault() ?? "Local guide",
                    booking.BookingType == BookingType.Experience ? booking.Experience!.CulturalArea.Name : null,
                    booking.Destination,
                    booking.ScheduledStart,
                    booking.ScheduledEnd,
                    booking.NumberOfGuests,
                    booking.GuideHours,
                    booking.ServiceAmount,
                    booking.PlatformFeeRate,
                    booking.PlatformFeeAmount,
                    booking.TotalAmount,
                    booking.Status,
                    booking.Payment.Status,
                    booking.Payment.Amount,
                    booking.Refunds.Sum(refund => (decimal?)refund.Amount) ?? 0m,
                    booking.Refunds.OrderBy(refund => refund.CreatedAt)
                        .Select(refund => new TouristBookingRefundResponse(refund.Amount, refund.Reason, refund.CreatedAt))
                        .ToList(),
                    booking.Payout == null ? null : booking.Payout.Amount,
                    booking.Payout == null ? null : booking.Payout.Status,
                    booking.Payout == null ? null : booking.Payout.EligibleAt,
                    booking.Disputes.Any(dispute => dispute.Status == DisputeStatus.Open || dispute.Status == DisputeStatus.UnderReview),
                    false,
                    false,
                    false,
                    booking.Review == null ? null : booking.Review.Rating,
                    booking.Review == null ? null : booking.Review.Comment,
                    booking.Review == null ? null : booking.Review.CreatedAt,
                    false,
                    booking.Payment.Status == PaymentStatus.Paid
                        && (booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.Completed)
                        && booking.BookingType == BookingType.Experience
                            ? booking.Experience!.ExactMeetingPoint
                            : null))
                .SingleOrDefaultAsync(cancellationToken);

        public async Task<BookingCancellationOutcome> CancelAsync(
            string userId,
            int bookingId,
            BookingStatus expectedStatus,
            decimal refundAmount,
            decimal providerAmount,
            PaymentStatus? updatedPaymentStatus,
            DateTime nowUtc,
            CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var booking = await _dbContext.Bookings
                .Include(item => item.Payment)
                .Include(item => item.Payout)
                .Include(item => item.Refunds)
                .Where(item => item.TouristUserId == userId)
                .SingleOrDefaultAsync(item => item.Id == bookingId, cancellationToken);
            if (booking is null) return BookingCancellationOutcome.BookingUnavailable;
            if (booking.Status != expectedStatus
                || (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
                || nowUtc >= DateTime.SpecifyKind(booking.ScheduledStart, DateTimeKind.Utc))
                return BookingCancellationOutcome.NotCancellable;
            if (refundAmount <= 0m || refundAmount > booking.ServiceAmount || providerAmount < 0m)
                return BookingCancellationOutcome.NotCancellable;
            if (providerAmount > 0m && booking.Payout is not null && booking.Payout.Status != PayoutStatus.Pending)
                return BookingCancellationOutcome.PayoutUnavailable;

            var cancelledStatus = booking.Status;
            booking.Status = BookingStatus.Cancelled;
            if (updatedPaymentStatus.HasValue)
                booking.Payment.Status = updatedPaymentStatus.Value;

            var reason = cancelledStatus == BookingStatus.Pending
                ? "Tourist cancelled a pending booking. The full service amount was recorded as a simulated refund; the platform fee is not refundable."
                : providerAmount > 0m
                    ? "Tourist cancelled a confirmed booking less than 48 hours before start. Half of the service amount was recorded as a simulated refund and half allocated to the provider; the platform fee is not refundable."
                    : "Tourist cancelled a confirmed booking at least 48 hours before start. The full service amount was recorded as a simulated refund; the platform fee is not refundable.";

            _dbContext.Refunds.Add(new Refund
            {
                BookingId = booking.Id,
                Amount = refundAmount,
                Reason = reason,
                CreatedAt = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc)
            });

            if (providerAmount > 0m)
            {
                if (booking.Payout is null)
                {
                    booking.Payout = new Payout
                    {
                        BookingId = booking.Id,
                        Amount = providerAmount,
                        EligibleAt = DateTime.SpecifyKind(booking.ScheduledEnd, DateTimeKind.Utc).AddHours(24),
                        Status = PayoutStatus.Pending
                    };
                }
                else
                {
                    booking.Payout.Amount = providerAmount;
                    booking.Payout.EligibleAt = DateTime.SpecifyKind(booking.ScheduledEnd, DateTimeKind.Utc).AddHours(24);
                    booking.Payout.Status = PayoutStatus.Pending;
                }
            }
            else if (booking.Payout is not null && booking.Payout.Status == PayoutStatus.Pending)
            {
                booking.Payout.Amount = 0m;
                booking.Payout.Status = PayoutStatus.Cancelled;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return BookingCancellationOutcome.Success;
        }
    }
}
