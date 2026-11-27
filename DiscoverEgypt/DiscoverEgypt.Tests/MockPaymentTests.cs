using DiscoverEgypt.Application.Contracts.Payments;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Payments;
using DiscoverEgypt.Application.Features.Payments.Services;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;
using FluentValidation;

namespace DiscoverEgypt.Tests
{
    public sealed class MockPaymentTests
    {
        private static readonly DateTimeOffset Now = new(2026, 11, 23, 12, 0, 0, TimeSpan.Zero);

        [Fact]
        public async Task CreateCheckoutSession_SavesSessionId_AndReturnsStripeUrl()
        {
            var repository = new FakeTouristPaymentRepository();
            var gateway = new FakeStripePaymentGateway();
            var service = CreateService(repository, gateway);

            var created = await service.CreateCheckoutSessionAsync(
                "tourist-user-id",
                31,
                "https://localhost/MockPayments/Success?bookingId=31&session_id={CHECKOUT_SESSION_ID}",
                "https://localhost/MockPayments/Cancel?bookingId=31",
                CancellationToken.None);

            Assert.Equal("cs_test_123", created.SessionId);
            Assert.Equal("https://checkout.stripe.com/c/pay/cs_test_123", created.CheckoutUrl);
            Assert.Equal("cs_test_123", repository.Payment.StripeSessionId);
            Assert.Equal(PaymentStatus.Pending, repository.Payment.Status);
            Assert.Equal(1150m, gateway.LastRequest!.Amount);
        }

        [Fact]
        public async Task CompleteCheckout_WhenStripePaid_SetsPaidAtUtc_AndLeavesBookingPendingAndAmountsUnchanged()
        {
            var repository = new FakeTouristPaymentRepository();
            repository.Payment.StripeSessionId = "cs_test_123";
            var gateway = new FakeStripePaymentGateway
            {
                SessionStatus = new StripeCheckoutSessionStatus(
                    "cs_test_123", "pi_test_999", 31, "tourist-user-id", IsPaid: true, IsExpiredOrFailed: false)
            };
            var service = CreateService(repository, gateway);

            var response = await service.CompleteCheckoutAsync("tourist-user-id", 31, "cs_test_123", CancellationToken.None);

            Assert.Equal(PaymentStatus.Paid, response.PaymentStatus);
            Assert.Equal(BookingStatus.Pending, response.BookingStatus);
            Assert.NotNull(response.PaidAtUtc);
            Assert.Equal(DateTimeKind.Utc, response.PaidAtUtc.Value.Kind);
            Assert.Equal(Now.UtcDateTime, response.PaidAtUtc.Value);
            Assert.Equal(1150m, response.Amount);
            Assert.Equal(PaymentStatus.Paid, repository.Payment.Status);
            Assert.Equal("pi_test_999", repository.Payment.StripePaymentIntentId);
            Assert.Equal(Now.UtcDateTime, repository.Payment.PaidAt!.Value);
            Assert.Equal(BookingStatus.Pending, repository.Booking.Status);
            Assert.Equal(1000m, repository.Booking.ServiceAmount);
            Assert.Equal(0.15m, repository.Booking.PlatformFeeRate);
            Assert.Equal(150m, repository.Booking.PlatformFeeAmount);
            Assert.Equal(1150m, repository.Booking.TotalAmount);
            Assert.Equal(1150m, repository.Payment.Amount);
        }

        [Fact]
        public async Task CompleteCheckout_WhenStripeExpired_SetsFailedWithoutPaidAt_AndLeavesBookingPending()
        {
            var repository = new FakeTouristPaymentRepository();
            repository.Payment.StripeSessionId = "cs_test_123";
            var gateway = new FakeStripePaymentGateway
            {
                SessionStatus = new StripeCheckoutSessionStatus(
                    "cs_test_123", null, 31, "tourist-user-id", IsPaid: false, IsExpiredOrFailed: true)
            };

            var response = await CreateService(repository, gateway).CompleteCheckoutAsync(
                "tourist-user-id", 31, "cs_test_123", CancellationToken.None);

            Assert.Equal(PaymentStatus.Failed, response.PaymentStatus);
            Assert.Null(response.PaidAtUtc);
            Assert.Equal(BookingStatus.Pending, response.BookingStatus);
            Assert.Equal(PaymentStatus.Failed, repository.Payment.Status);
            Assert.Null(repository.Payment.PaidAt);
            Assert.Equal(1150m, repository.Booking.TotalAmount);
        }

        [Fact]
        public async Task CreateCheckoutSession_RejectsAnotherTouristsPayment()
        {
            var repository = new FakeTouristPaymentRepository { OwnerUserId = "another-tourist" };
            var gateway = new FakeStripePaymentGateway();

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                CreateService(repository, gateway).CreateCheckoutSessionAsync(
                    "tourist-user-id", 31, "https://localhost/s", "https://localhost/c", CancellationToken.None));

            Assert.Contains("unavailable", exception.Message, StringComparison.Ordinal);
            Assert.Equal(PaymentStatus.Pending, repository.Payment.Status);
            Assert.Null(gateway.LastRequest);
        }

        [Fact]
        public async Task CreateCheckoutSession_RejectsAlreadyPaidPayment()
        {
            var repository = new FakeTouristPaymentRepository();
            repository.Payment.Status = PaymentStatus.Paid;
            repository.Payment.PaidAt = Now.UtcDateTime;
            var gateway = new FakeStripePaymentGateway();

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                CreateService(repository, gateway).CreateCheckoutSessionAsync(
                    "tourist-user-id", 31, "https://localhost/s", "https://localhost/c", CancellationToken.None));

            Assert.Contains("already been completed", exception.Message, StringComparison.Ordinal);
            Assert.Null(gateway.LastRequest);
        }

        [Fact]
        public async Task CompleteCheckout_IsIdempotent_WhenAlreadyPaidForSameSession()
        {
            var repository = new FakeTouristPaymentRepository();
            repository.Payment.Status = PaymentStatus.Paid;
            repository.Payment.PaidAt = Now.UtcDateTime;
            repository.Payment.StripeSessionId = "cs_test_123";
            var gateway = new FakeStripePaymentGateway();

            var response = await CreateService(repository, gateway).CompleteCheckoutAsync(
                "tourist-user-id", 31, "cs_test_123", CancellationToken.None);

            Assert.Equal(PaymentStatus.Paid, response.PaymentStatus);
            Assert.Equal(Now.UtcDateTime, response.PaidAtUtc);
        }

        [Fact]
        public async Task ProcessWebhook_WhenCheckoutCompletedPaid_MarksPaymentPaidUtc()
        {
            var repository = new FakeTouristPaymentRepository();
            var gateway = new FakeStripePaymentGateway
            {
                WebhookResult = new StripeWebhookEventResult(
                    StripeWebhookEventType.CheckoutCompleted,
                    31,
                    "tourist-user-id",
                    "cs_test_123",
                    "pi_test_999",
                    IsPaid: true)
            };

            var handled = await CreateService(repository, gateway).ProcessWebhookAsync(
                "{}", "t=1,v1=sig", CancellationToken.None);

            Assert.True(handled);
            Assert.Equal(PaymentStatus.Paid, repository.Payment.Status);
            Assert.Equal(Now.UtcDateTime, repository.Payment.PaidAt);
            Assert.Equal("cs_test_123", repository.Payment.StripeSessionId);
            Assert.Equal("pi_test_999", repository.Payment.StripePaymentIntentId);
        }

        private static MockPaymentService CreateService(
            FakeTouristPaymentRepository repository,
            FakeStripePaymentGateway gateway) =>
            new(repository, gateway, new FixedTimeProvider(Now));

        private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => now;
        }

        private sealed class FakeStripePaymentGateway : IStripePaymentGateway
        {
            public StripeCheckoutSessionRequest? LastRequest { get; private set; }
            public StripeCheckoutSessionStatus SessionStatus { get; init; } = new(
                "cs_test_123", "pi_test_123", 31, "tourist-user-id", IsPaid: true, IsExpiredOrFailed: false);
            public StripeWebhookEventResult WebhookResult { get; init; } = new(
                StripeWebhookEventType.Ignored, null, null, null, null, false);

            public Task<StripeCheckoutSessionCreated> CreateCheckoutSessionAsync(
                StripeCheckoutSessionRequest request,
                CancellationToken cancellationToken = default)
            {
                LastRequest = request;
                return Task.FromResult(new StripeCheckoutSessionCreated(
                    "cs_test_123",
                    "https://checkout.stripe.com/c/pay/cs_test_123"));
            }

            public Task<StripeCheckoutSessionStatus> GetCheckoutSessionAsync(
                string sessionId,
                CancellationToken cancellationToken = default) =>
                Task.FromResult(SessionStatus);

            public StripeWebhookEventResult ParseWebhookEvent(string payload, string signatureHeader) =>
                WebhookResult;
        }

        private sealed class FakeTouristPaymentRepository : ITouristPaymentRepository
        {
            public string OwnerUserId { get; init; } = "tourist-user-id";
            public Booking Booking { get; } = new()
            {
                TouristUserId = "tourist-user-id",
                ProviderUserId = "host-user-id",
                BookingType = BookingType.Experience,
                ExperienceId = 12,
                ScheduledStart = Now.AddDays(5).UtcDateTime,
                ScheduledEnd = Now.AddDays(5).AddHours(2).UtcDateTime,
                NumberOfGuests = 2,
                PricePerPersonSnapshot = 500m,
                ServiceAmount = 1000m,
                PlatformFeeRate = 0.15m,
                PlatformFeeAmount = 150m,
                TotalAmount = 1150m,
                Status = BookingStatus.Pending,
                CreatedAt = Now.UtcDateTime
            };
            public Payment Payment { get; } = new()
            {
                BookingId = 31,
                Amount = 1150m,
                Status = PaymentStatus.Pending,
                CreatedAt = Now.UtcDateTime
            };

            public Task<MockPaymentResponse?> GetOwnedPaymentAsync(string userId, int bookingId, CancellationToken cancellationToken = default)
            {
                if (userId != OwnerUserId || bookingId != 31) return Task.FromResult<MockPaymentResponse?>(null);
                return Task.FromResult<MockPaymentResponse?>(new MockPaymentResponse(
                    bookingId, Booking.BookingType, "Cairo Food Walk", Booking.ScheduledStart,
                    Payment.Amount, Booking.Status, Payment.Status, Payment.PaidAt, Payment.StripeSessionId));
            }

            public Task<bool> SaveStripeSessionAsync(
                string userId,
                int bookingId,
                string stripeSessionId,
                CancellationToken cancellationToken = default)
            {
                if (userId != OwnerUserId || bookingId != 31 || Booking.Status != BookingStatus.Pending
                    || (Payment.Status != PaymentStatus.Pending && Payment.Status != PaymentStatus.Failed))
                {
                    return Task.FromResult(false);
                }

                Payment.StripeSessionId = stripeSessionId;
                return Task.FromResult(true);
            }

            public Task<bool> TryCompletePendingPaymentAsync(
                string userId,
                int bookingId,
                PaymentStatus status,
                DateTime? paidAtUtc,
                string? stripeSessionId = null,
                string? stripePaymentIntentId = null,
                CancellationToken cancellationToken = default)
            {
                if (userId != OwnerUserId || bookingId != 31 || Booking.Status != BookingStatus.Pending
                    || (Payment.Status != PaymentStatus.Pending && Payment.Status != PaymentStatus.Failed))
                {
                    return Task.FromResult(false);
                }

                Payment.Status = status;
                Payment.PaidAt = paidAtUtc;
                if (!string.IsNullOrWhiteSpace(stripeSessionId)) Payment.StripeSessionId = stripeSessionId;
                if (!string.IsNullOrWhiteSpace(stripePaymentIntentId)) Payment.StripePaymentIntentId = stripePaymentIntentId;
                return Task.FromResult(true);
            }

            public Task<bool> TryCompletePaymentFromWebhookAsync(
                int bookingId,
                PaymentStatus status,
                DateTime? paidAtUtc,
                string? stripeSessionId,
                string? stripePaymentIntentId,
                CancellationToken cancellationToken = default)
            {
                if (bookingId != 31 || Booking.Status != BookingStatus.Pending
                    || (Payment.Status != PaymentStatus.Pending && Payment.Status != PaymentStatus.Failed))
                {
                    return Task.FromResult(false);
                }

                Payment.Status = status;
                Payment.PaidAt = paidAtUtc;
                if (!string.IsNullOrWhiteSpace(stripeSessionId)) Payment.StripeSessionId = stripeSessionId;
                if (!string.IsNullOrWhiteSpace(stripePaymentIntentId)) Payment.StripePaymentIntentId = stripePaymentIntentId;
                return Task.FromResult(true);
            }
        }
    }
}
