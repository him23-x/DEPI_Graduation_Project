using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Bookings;
using DiscoverEgypt.Application.Features.Disputes.Policies;
using DiscoverEgypt.Application.Features.Reviews.Policies;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Features.Bookings.Services
{
    public sealed class TouristBookingService
    {
        private static readonly TimeSpan CancellationBoundary = TimeSpan.FromHours(48);
        private readonly ITouristBookingRepository _repository;
        private readonly TimeProvider _timeProvider;

        public TouristBookingService(ITouristBookingRepository repository, TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<IReadOnlyList<TouristBookingListItemResponse>> GetForTouristAsync(
            string userId,
            CancellationToken cancellationToken)
        {
            var items = await _repository.GetForTouristAsync(userId, cancellationToken);
            return items.Select(item => item with
            {
                ScheduledStartUtc = AsUtc(item.ScheduledStartUtc),
                ScheduledEndUtc = AsUtc(item.ScheduledEndUtc)
            }).ToArray();
        }

        public async Task<TouristBookingDetailsResponse?> GetDetailsAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken)
        {
            var details = await _repository.GetDetailsForTouristAsync(userId, bookingId, cancellationToken);
            if (details is null) return null;

            var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
            return details with
            {
                ScheduledStartUtc = AsUtc(details.ScheduledStartUtc),
                ScheduledEndUtc = AsUtc(details.ScheduledEndUtc),
                PayoutEligibleAtUtc = details.PayoutEligibleAtUtc.HasValue
                    ? AsUtc(details.PayoutEligibleAtUtc.Value)
                    : null,
                Refunds = details.Refunds.Select(refund => refund with { CreatedAtUtc = AsUtc(refund.CreatedAtUtc) }).ToArray(),
                CanReview = TouristReviewEligibilityPolicy.CanReview(details.BookingStatus, details.ReviewRating.HasValue),
                ReviewCreatedAtUtc = details.ReviewCreatedAtUtc.HasValue ? AsUtc(details.ReviewCreatedAtUtc.Value) : null,
                CanShareBooking = details.BookingStatus == BookingStatus.Confirmed
                    && details.PaymentStatus == PaymentStatus.Paid
                    && AsUtc(details.ScheduledEndUtc) > nowUtc
                    && !string.IsNullOrWhiteSpace(details.BookingType == Domain.Enums.Bookings.BookingType.Experience
                        ? details.ExactMeetingPoint
                        : details.Destination),
                ExactMeetingPoint = (details.BookingStatus is BookingStatus.Confirmed or BookingStatus.Completed)
                    && details.PaymentStatus == PaymentStatus.Paid
                    ? details.ExactMeetingPoint
                    : null,
                CanCancel = IsCancellable(details.BookingStatus, AsUtc(details.ScheduledStartUtc), nowUtc),
                CanOpenDispute = TouristDisputeEligibilityPolicy.CanOpen(
                    details.BookingStatus,
                    AsUtc(details.ScheduledEndUtc),
                    details.PayoutAmount,
                    nowUtc)
            };
        }

        public async Task<BookingCancellationOutcome> CancelAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            var details = await _repository.GetDetailsForTouristAsync(userId, bookingId, cancellationToken);
            if (details is null) return BookingCancellationOutcome.BookingUnavailable;

            var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
            var scheduledStartUtc = AsUtc(details.ScheduledStartUtc);
            if (!IsCancellable(details.BookingStatus, scheduledStartUtc, nowUtc))
                return BookingCancellationOutcome.NotCancellable;

            decimal refundAmount;
            decimal providerAmount;
            if (details.BookingStatus == BookingStatus.Pending)
            {
                refundAmount = details.ServiceAmount;
                providerAmount = 0m;
            }
            else if (scheduledStartUtc - nowUtc >= CancellationBoundary)
            {
                refundAmount = details.ServiceAmount;
                providerAmount = 0m;
            }
            else
            {
                refundAmount = decimal.Round(details.ServiceAmount / 2m, 2, MidpointRounding.AwayFromZero);
                providerAmount = details.ServiceAmount - refundAmount;
            }

            PaymentStatus? updatedPaymentStatus = details.PaymentStatus switch
            {
                PaymentStatus.Paid or PaymentStatus.PartiallyRefunded or PaymentStatus.Refunded =>
                    providerAmount > 0m ? PaymentStatus.PartiallyRefunded : PaymentStatus.Refunded,
                _ => null
            };

            return await _repository.CancelAsync(
                userId,
                bookingId,
                details.BookingStatus,
                refundAmount,
                providerAmount,
                updatedPaymentStatus,
                nowUtc,
                cancellationToken);
        }

        private static bool IsCancellable(BookingStatus status, DateTime scheduledStartUtc, DateTime nowUtc) =>
            (status == BookingStatus.Pending || status == BookingStatus.Confirmed)
            && nowUtc < scheduledStartUtc;

        private static DateTime AsUtc(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}
