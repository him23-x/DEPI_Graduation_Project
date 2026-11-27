using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Services;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Tests
{
    public sealed class TouristBookingCancellationTests
    {
        private static readonly DateTimeOffset Now = new(2026, 11, 23, 12, 0, 0, TimeSpan.Zero);

        [Fact]
        public async Task CancelPendingBooking_RecordsFullServiceRefund_AndLeavesPaymentStatusSeparate()
        {
            var repository = new FakeTouristBookingRepository(CreateBooking(BookingStatus.Pending, Now.AddHours(24)))
            {
                PaymentStatus = PaymentStatus.Pending
            };

            var outcome = await CreateService(repository).CancelAsync("tourist-user-id", 51, CancellationToken.None);

            Assert.Equal(BookingCancellationOutcome.Success, outcome);
            Assert.Equal(500m, repository.RefundAmount);
            Assert.Equal(0m, repository.ProviderAmount);
            Assert.Equal(BookingStatus.Cancelled, repository.BookingStatus);
            Assert.Equal(PaymentStatus.Pending, repository.PaymentStatus);
            Assert.Equal(500m, repository.ServiceAmount);
            Assert.Equal(0.15m, repository.PlatformFeeRate);
            Assert.Equal(75m, repository.PlatformFeeAmount);
            Assert.Equal(575m, repository.TotalAmount);
        }

        [Fact]
        public async Task CancelConfirmedBooking_AtExact48Hours_RefundsFullServiceAndCancelsPayout()
        {
            var repository = new FakeTouristBookingRepository(CreateBooking(BookingStatus.Confirmed, Now.AddHours(48)))
            {
                PaymentStatus = PaymentStatus.Paid,
                PayoutAmount = 500m,
                PayoutStatus = PayoutStatus.Pending
            };

            var outcome = await CreateService(repository).CancelAsync("tourist-user-id", 51, CancellationToken.None);

            Assert.Equal(BookingCancellationOutcome.Success, outcome);
            Assert.Equal(500m, repository.RefundAmount);
            Assert.Equal(0m, repository.ProviderAmount);
            Assert.Equal(PaymentStatus.Refunded, repository.PaymentStatus);
            Assert.Equal(PayoutStatus.Cancelled, repository.PayoutStatus);
            Assert.Equal(0m, repository.PayoutAmount);
        }

        [Fact]
        public async Task CancelConfirmedBooking_JustUnder48Hours_SplitsServiceAndKeepsPayoutPendingUntilEligible()
        {
            var scheduledStart = Now.AddHours(47).AddMinutes(59);
            var scheduledEnd = scheduledStart.AddHours(2);
            var repository = new FakeTouristBookingRepository(CreateBooking(BookingStatus.Confirmed, scheduledStart, scheduledEnd))
            {
                PaymentStatus = PaymentStatus.Paid,
                PayoutAmount = 500m,
                PayoutStatus = PayoutStatus.Pending,
                HasOpenDispute = true
            };

            var outcome = await CreateService(repository).CancelAsync("tourist-user-id", 51, CancellationToken.None);

            Assert.Equal(BookingCancellationOutcome.Success, outcome);
            Assert.Equal(250m, repository.RefundAmount);
            Assert.Equal(250m, repository.ProviderAmount);
            Assert.Equal(PaymentStatus.PartiallyRefunded, repository.PaymentStatus);
            Assert.Equal(PayoutStatus.Pending, repository.PayoutStatus);
            Assert.Equal(scheduledEnd.AddHours(24).UtcDateTime, repository.PayoutEligibleAtUtc!.Value);
            Assert.True(repository.HasOpenDispute);
        }

        [Fact]
        public async Task Cancel_RejectsAnotherTouristsBookingAndNonCancellableStatus()
        {
            var wrongOwner = new FakeTouristBookingRepository(CreateBooking(BookingStatus.Pending, Now.AddHours(80)))
            {
                OwnerUserId = "another-tourist"
            };
            var unavailable = await CreateService(wrongOwner).CancelAsync("tourist-user-id", 51, CancellationToken.None);
            Assert.Equal(BookingCancellationOutcome.BookingUnavailable, unavailable);

            var completed = new FakeTouristBookingRepository(CreateBooking(BookingStatus.Completed, Now.AddHours(80)));
            var notCancellable = await CreateService(completed).CancelAsync("tourist-user-id", 51, CancellationToken.None);
            Assert.Equal(BookingCancellationOutcome.NotCancellable, notCancellable);
        }

        [Fact]
        public void TouristBookingListDoesNotExposeExactMeetingPoint_BookingDetailsExposeItOnlyForEligibleBookings()
        {
            Assert.Null(typeof(TouristBookingListItemResponse).GetProperty("ExactMeetingPoint"));
            Assert.NotNull(typeof(TouristBookingDetailsResponse).GetProperty("ExactMeetingPoint"));
        }

        private static TouristBookingService CreateService(FakeTouristBookingRepository repository) =>
            new(repository, new FixedTimeProvider(Now));

        private static TouristBookingDetailsResponse CreateBooking(
            BookingStatus status,
            DateTimeOffset scheduledStart,
            DateTimeOffset? scheduledEnd = null) => new(
                51,
                BookingType.Experience,
                "Cairo Food Walk",
                "Old Cairo",
                null,
                scheduledStart.UtcDateTime,
                (scheduledEnd ?? scheduledStart.AddHours(2)).UtcDateTime,
                2,
                null,
                500m,
                0.15m,
                75m,
                575m,
                status,
                PaymentStatus.Paid,
                575m,
                0m,
                Array.Empty<TouristBookingRefundResponse>(),
                status == BookingStatus.Confirmed ? 500m : null,
                status == BookingStatus.Confirmed ? PayoutStatus.Pending : null,
                status == BookingStatus.Confirmed ? scheduledEnd?.AddHours(24).UtcDateTime ?? scheduledStart.AddHours(26).UtcDateTime : null,
                false,
                false,
                false,
                false,
                null,
                null,
                null,
                false,
                null);

        private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => now;
        }

        private sealed class FakeTouristBookingRepository(TouristBookingDetailsResponse booking) : ITouristBookingRepository
        {
            public string OwnerUserId { get; init; } = "tourist-user-id";
            public PaymentStatus PaymentStatus { get; set; } = booking.PaymentStatus;
            public BookingStatus BookingStatus { get; private set; } = booking.BookingStatus;
            public decimal RefundAmount { get; private set; }
            public decimal ProviderAmount { get; private set; }
            public decimal ServiceAmount => booking.ServiceAmount;
            public decimal PlatformFeeRate => booking.PlatformFeeRate;
            public decimal PlatformFeeAmount => booking.PlatformFeeAmount;
            public decimal TotalAmount => booking.TotalAmount;
            public decimal? PayoutAmount { get; set; } = booking.PayoutAmount;
            public PayoutStatus? PayoutStatus { get; set; } = booking.PayoutStatus;
            public DateTime? PayoutEligibleAtUtc { get; private set; } = booking.PayoutEligibleAtUtc;
            public bool HasOpenDispute { get; set; } = booking.HasOpenDispute;

            public Task<IReadOnlyList<TouristBookingListItemResponse>> GetForTouristAsync(string userId, CancellationToken cancellationToken = default) =>
                Task.FromResult<IReadOnlyList<TouristBookingListItemResponse>>(Array.Empty<TouristBookingListItemResponse>());

            public Task<TouristBookingDetailsResponse?> GetDetailsForTouristAsync(string userId, int bookingId, CancellationToken cancellationToken = default) =>
                Task.FromResult<TouristBookingDetailsResponse?>(userId == OwnerUserId && bookingId == booking.BookingId
                    ? booking with { BookingStatus = BookingStatus, PaymentStatus = PaymentStatus, PayoutAmount = PayoutAmount, PayoutStatus = PayoutStatus, PayoutEligibleAtUtc = PayoutEligibleAtUtc }
                    : null);

            public Task<BookingCancellationOutcome> CancelAsync(
                string userId,
                int bookingId,
                BookingStatus expectedStatus,
                decimal refundAmount,
                decimal providerAmount,
                PaymentStatus? updatedPaymentStatus,
                DateTime nowUtc,
                CancellationToken cancellationToken = default)
            {
                if (userId != OwnerUserId || bookingId != booking.BookingId) return Task.FromResult(BookingCancellationOutcome.BookingUnavailable);
                if (BookingStatus != expectedStatus || (BookingStatus != DiscoverEgypt.Domain.Enums.Bookings.BookingStatus.Pending && BookingStatus != DiscoverEgypt.Domain.Enums.Bookings.BookingStatus.Confirmed))
                    return Task.FromResult(BookingCancellationOutcome.NotCancellable);
                RefundAmount = refundAmount;
                ProviderAmount = providerAmount;
                BookingStatus = DiscoverEgypt.Domain.Enums.Bookings.BookingStatus.Cancelled;
                if (updatedPaymentStatus.HasValue) PaymentStatus = updatedPaymentStatus.Value;
                if (PayoutStatus == DiscoverEgypt.Domain.Enums.Payments.PayoutStatus.Pending)
                {
                    PayoutAmount = providerAmount;
                    PayoutStatus = providerAmount > 0m
                        ? DiscoverEgypt.Domain.Enums.Payments.PayoutStatus.Pending
                        : DiscoverEgypt.Domain.Enums.Payments.PayoutStatus.Cancelled;
                }
                PayoutEligibleAtUtc ??= booking.ScheduledEndUtc.AddHours(24);
                return Task.FromResult(BookingCancellationOutcome.Success);
            }
        }
    }
}
