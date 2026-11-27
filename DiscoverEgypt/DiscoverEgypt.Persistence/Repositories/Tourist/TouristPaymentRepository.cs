using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Tourist
{
    public sealed class TouristPaymentRepository : ITouristPaymentRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public TouristPaymentRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public Task<MockPaymentResponse?> GetOwnedPaymentAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken = default) =>
            _dbContext.Bookings.AsNoTracking()
                .Where(booking => booking.Id == bookingId
                    && booking.TouristUserId == userId
                    && booking.Payment != null
                    && booking.Payment.Amount == booking.TotalAmount
                    && (booking.BookingType == BookingType.Experience || booking.BookingType == BookingType.Guide))
                .Select(booking => new MockPaymentResponse(
                    booking.Id,
                    booking.BookingType,
                    booking.BookingType == BookingType.Experience
                        ? booking.Experience!.Title
                        : _dbContext.ApplicationUsers.Where(user => user.Id == booking.GuideProfile!.UserId)
                            .Select(user => user.FullName).FirstOrDefault() ?? "Local guide",
                    booking.ScheduledStart,
                    booking.TotalAmount,
                    booking.Status,
                    booking.Payment.Status,
                    booking.Payment.PaidAt,
                    booking.Payment.StripeSessionId))
                .SingleOrDefaultAsync(cancellationToken);

        public async Task<bool> SaveStripeSessionAsync(
            string userId,
            int bookingId,
            string stripeSessionId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(stripeSessionId);

            var rows = await _dbContext.Payments
                .Where(payment => payment.BookingId == bookingId
                    && payment.Booking.TouristUserId == userId
                    && payment.Booking.Status == BookingStatus.Pending
                    && (payment.Status == PaymentStatus.Pending || payment.Status == PaymentStatus.Failed))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(payment => payment.StripeSessionId, stripeSessionId), cancellationToken);

            return rows == 1;
        }

        public async Task<bool> TryCompletePendingPaymentAsync(
            string userId,
            int bookingId,
            PaymentStatus status,
            DateTime? paidAtUtc,
            string? stripeSessionId = null,
            string? stripePaymentIntentId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateOutcomeArguments(status, paidAtUtc);

            var rows = await _dbContext.Payments
                .Where(payment => payment.BookingId == bookingId
                    && payment.Booking.TouristUserId == userId
                    && payment.Booking.Status == BookingStatus.Pending
                    && (payment.Status == PaymentStatus.Pending || payment.Status == PaymentStatus.Failed))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(payment => payment.Status, status)
                    .SetProperty(payment => payment.PaidAt, paidAtUtc)
                    .SetProperty(payment => payment.StripeSessionId, payment => stripeSessionId ?? payment.StripeSessionId)
                    .SetProperty(payment => payment.StripePaymentIntentId, payment => stripePaymentIntentId ?? payment.StripePaymentIntentId), cancellationToken);

            return rows == 1;
        }

        public async Task<bool> TryCompletePaymentFromWebhookAsync(
            int bookingId,
            PaymentStatus status,
            DateTime? paidAtUtc,
            string? stripeSessionId,
            string? stripePaymentIntentId,
            CancellationToken cancellationToken = default)
        {
            ValidateOutcomeArguments(status, paidAtUtc);

            var rows = await _dbContext.Payments
                .Where(payment => payment.BookingId == bookingId
                    && payment.Booking.Status == BookingStatus.Pending
                    && (payment.Status == PaymentStatus.Pending || payment.Status == PaymentStatus.Failed))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(payment => payment.Status, status)
                    .SetProperty(payment => payment.PaidAt, paidAtUtc)
                    .SetProperty(payment => payment.StripeSessionId, payment => stripeSessionId ?? payment.StripeSessionId)
                    .SetProperty(payment => payment.StripePaymentIntentId, payment => stripePaymentIntentId ?? payment.StripePaymentIntentId), cancellationToken);

            return rows == 1;
        }

        private static void ValidateOutcomeArguments(PaymentStatus status, DateTime? paidAtUtc)
        {
            if (status is not (PaymentStatus.Paid or PaymentStatus.Failed))
                throw new ArgumentOutOfRangeException(nameof(status), "Only Paid or Failed payment outcomes are supported.");
            if (status == PaymentStatus.Paid && paidAtUtc?.Kind != DateTimeKind.Utc)
                throw new ArgumentException("PaidAt must be stored as UTC.", nameof(paidAtUtc));
            if (status == PaymentStatus.Failed && paidAtUtc.HasValue)
                throw new ArgumentException("Failed payments cannot have a PaidAt timestamp.", nameof(paidAtUtc));
        }
    }
}
