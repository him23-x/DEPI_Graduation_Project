using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Disputes;
using DiscoverEgypt.Application.Features.Disputes.Policies;
using DiscoverEgypt.Application.Features.Disputes.Services;
using DiscoverEgypt.Application.Responses.Disputes;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;

namespace DiscoverEgypt.Tests
{
    public sealed class TouristDisputeServiceTests
    {
        private static readonly DateTimeOffset Now = new(2026, 11, 23, 12, 0, 0, TimeSpan.Zero);

        [Fact]
        public void ConfirmedAndCompletedBookings_AreEligibleThroughExact24HourBoundary()
        {
            var scheduledEnd = Now.AddHours(-24).UtcDateTime;

            Assert.True(TouristDisputeEligibilityPolicy.CanOpen(BookingStatus.Confirmed, scheduledEnd, null, Now.UtcDateTime));
            Assert.True(TouristDisputeEligibilityPolicy.CanOpen(BookingStatus.Completed, scheduledEnd, null, Now.UtcDateTime));
            Assert.False(TouristDisputeEligibilityPolicy.CanOpen(BookingStatus.Completed, scheduledEnd.AddTicks(-1), null, Now.UtcDateTime));
        }

        [Fact]
        public void CancelledBooking_RequiresPositiveProviderAllocation()
        {
            var scheduledEnd = Now.AddDays(-30).UtcDateTime;

            Assert.True(TouristDisputeEligibilityPolicy.CanOpen(BookingStatus.Cancelled, scheduledEnd, 125m, Now.UtcDateTime));
            Assert.False(TouristDisputeEligibilityPolicy.CanOpen(BookingStatus.Cancelled, scheduledEnd, 0m, Now.UtcDateTime));
            Assert.False(TouristDisputeEligibilityPolicy.CanOpen(BookingStatus.Cancelled, scheduledEnd, null, Now.UtcDateTime));
        }

        [Theory]
        [InlineData(BookingStatus.Pending)]
        [InlineData(BookingStatus.Rejected)]
        public void OtherBookingStates_AreNotEligible(BookingStatus status)
        {
            Assert.False(TouristDisputeEligibilityPolicy.CanOpen(
                status,
                Now.AddHours(-1).UtcDateTime,
                100m,
                Now.UtcDateTime));
        }

        [Fact]
        public async Task GetEligibleBooking_DoesNotExposeAnotherTouristsBooking()
        {
            var repository = new FakeTouristDisputeRepository(CreateBooking(BookingStatus.Confirmed, Now.AddHours(2)));
            var service = CreateService(repository);

            var booking = await service.GetEligibleBookingAsync("someone-else", 41, CancellationToken.None);

            Assert.Null(booking);
        }

        [Fact]
        public async Task Open_TrimsDescriptionAndStoresUtcTimeWithoutChangingPaymentOrPayout()
        {
            var repository = new FakeTouristDisputeRepository(CreateBooking(BookingStatus.Confirmed, Now.AddHours(3)))
            {
                PaymentStatus = "Paid",
                PayoutStatus = "Pending"
            };
            var service = CreateService(repository);

            var response = await service.OpenAsync("tourist-1", 41, "  The host did not arrive.  ", CancellationToken.None);

            Assert.Equal(DisputeOpenOutcome.Created, response.Outcome);
            Assert.Equal(73, response.DisputeId);
            Assert.Equal("The host did not arrive.", repository.CreatedDescription);
            Assert.Equal(DateTimeKind.Utc, repository.CreatedAtUtc?.Kind);
            Assert.Equal("Paid", repository.PaymentStatus);
            Assert.Equal("Pending", repository.PayoutStatus);
        }

        private static TouristDisputeService CreateService(FakeTouristDisputeRepository repository) =>
            new(repository, new FixedTimeProvider(Now));

        private static TouristDisputeBookingResponse CreateBooking(BookingStatus status, DateTimeOffset scheduledEnd) =>
            new(41, BookingType.Experience, "Cairo Food Walk", scheduledEnd.AddHours(-2).UtcDateTime,
                scheduledEnd.UtcDateTime, status, status == BookingStatus.Cancelled ? 125m : null);

        private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => now;
        }

        private sealed class FakeTouristDisputeRepository(TouristDisputeBookingResponse booking) : ITouristDisputeRepository
        {
            private const string Owner = "tourist-1";
            public string? CreatedDescription { get; private set; }
            public DateTime? CreatedAtUtc { get; private set; }
            public string? PaymentStatus { get; init; }
            public string? PayoutStatus { get; init; }

            public Task<TouristDisputeBookingResponse?> GetOwnedBookingAsync(
                string userId,
                int bookingId,
                CancellationToken cancellationToken = default) =>
                Task.FromResult<TouristDisputeBookingResponse?>(userId == Owner && bookingId == booking.BookingId ? booking : null);

            public Task<TouristDisputeOpenResponse> OpenAsync(
                string userId,
                int bookingId,
                string description,
                DateTime nowUtc,
                CancellationToken cancellationToken = default)
            {
                CreatedDescription = description;
                CreatedAtUtc = nowUtc;
                return Task.FromResult(new TouristDisputeOpenResponse(DisputeOpenOutcome.Created, 73));
            }

            public Task<IReadOnlyList<TouristDisputeListItemResponse>> GetForTouristAsync(
                string userId,
                CancellationToken cancellationToken = default) =>
                Task.FromResult<IReadOnlyList<TouristDisputeListItemResponse>>(Array.Empty<TouristDisputeListItemResponse>());

            public Task<TouristDisputeDetailsResponse?> GetDetailsForTouristAsync(
                string userId,
                int disputeId,
                CancellationToken cancellationToken = default) =>
                Task.FromResult<TouristDisputeDetailsResponse?>(null);
        }
    }
}
