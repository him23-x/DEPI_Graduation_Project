using DiscoverEgypt.Application.Contracts.Payments;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Payments;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Payments.Services
{
    public sealed class MockPaymentService
    {
        private readonly ITouristPaymentRepository _repository;
        private readonly IStripePaymentGateway _stripeGateway;
        private readonly TimeProvider _timeProvider;

        public MockPaymentService(
            ITouristPaymentRepository repository,
            IStripePaymentGateway stripeGateway,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _stripeGateway = stripeGateway;
            _timeProvider = timeProvider;
        }

        public async Task<MockPaymentResponse?> GetAsync(string userId, int bookingId, CancellationToken cancellationToken)
        {
            var response = await _repository.GetOwnedPaymentAsync(userId, bookingId, cancellationToken);
            return response is null ? null : SpecifyUtc(response);
        }

        public async Task<StripeCheckoutSessionCreated> CreateCheckoutSessionAsync(
            string userId,
            int bookingId,
            string successUrl,
            string cancelUrl,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            ArgumentException.ThrowIfNullOrWhiteSpace(successUrl);
            ArgumentException.ThrowIfNullOrWhiteSpace(cancelUrl);

            var current = await _repository.GetOwnedPaymentAsync(userId, bookingId, cancellationToken);
            if (current is null)
                throw new ValidationException("This booking payment is unavailable.");

            if (current.PaymentStatus == PaymentStatus.Paid)
                throw new ValidationException("This payment has already been completed.");

            if (current.BookingStatus != BookingStatus.Pending
                || current.PaymentStatus is not (PaymentStatus.Pending or PaymentStatus.Failed))
            {
                throw new ValidationException("This booking is no longer eligible for payment.");
            }

            var created = await _stripeGateway.CreateCheckoutSessionAsync(
                new StripeCheckoutSessionRequest(
                    bookingId,
                    userId,
                    current.ServiceTitle,
                    current.Amount,
                    successUrl,
                    cancelUrl),
                cancellationToken);

            var saved = await _repository.SaveStripeSessionAsync(userId, bookingId, created.SessionId, cancellationToken);
            if (!saved)
                throw new ValidationException("This booking is no longer eligible for payment.");

            return created;
        }

        public async Task<MockPaymentResponse> CompleteCheckoutAsync(
            string userId,
            int bookingId,
            string sessionId,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);

            var current = await _repository.GetOwnedPaymentAsync(userId, bookingId, cancellationToken);
            if (current is null)
                throw new ValidationException("This booking payment is unavailable.");

            if (current.PaymentStatus == PaymentStatus.Paid)
            {
                if (string.IsNullOrWhiteSpace(current.StripeSessionId)
                    || string.Equals(current.StripeSessionId, sessionId, StringComparison.Ordinal))
                {
                    return SpecifyUtc(current);
                }

                throw new ValidationException("This booking has already been paid with another checkout session.");
            }

            if (current.BookingStatus != BookingStatus.Pending)
                throw new ValidationException("This booking is no longer eligible for payment.");

            if (!string.IsNullOrWhiteSpace(current.StripeSessionId)
                && !string.Equals(current.StripeSessionId, sessionId, StringComparison.Ordinal))
            {
                throw new ValidationException("The Stripe checkout session does not match this booking.");
            }

            var status = await _stripeGateway.GetCheckoutSessionAsync(sessionId, cancellationToken);
            if (status.BookingId != bookingId || !string.Equals(status.UserId, userId, StringComparison.Ordinal))
                throw new ValidationException("The Stripe checkout session does not match this booking.");

            if (status.IsPaid)
            {
                var paidAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
                var updated = await _repository.TryCompletePendingPaymentAsync(
                    userId,
                    bookingId,
                    PaymentStatus.Paid,
                    paidAtUtc,
                    status.SessionId,
                    status.PaymentIntentId,
                    cancellationToken);

                var result = await _repository.GetOwnedPaymentAsync(userId, bookingId, cancellationToken);
                if (result is null)
                    throw new ValidationException("This booking payment is unavailable.");

                if (!updated && result.PaymentStatus != PaymentStatus.Paid)
                    throw new ValidationException("This payment could not be completed.");

                return SpecifyUtc(result);
            }

            if (status.IsExpiredOrFailed)
            {
                await _repository.TryCompletePendingPaymentAsync(
                    userId,
                    bookingId,
                    PaymentStatus.Failed,
                    null,
                    status.SessionId,
                    status.PaymentIntentId,
                    cancellationToken);

                var failedResult = await _repository.GetOwnedPaymentAsync(userId, bookingId, cancellationToken);
                return failedResult is null
                    ? throw new ValidationException("This booking payment is unavailable.")
                    : SpecifyUtc(failedResult);
            }

            return SpecifyUtc(current);
        }

        public async Task<bool> ProcessWebhookAsync(
            string payload,
            string signatureHeader,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(payload);
            ArgumentException.ThrowIfNullOrWhiteSpace(signatureHeader);

            var webhookEvent = _stripeGateway.ParseWebhookEvent(payload, signatureHeader);
            if (webhookEvent.EventType == StripeWebhookEventType.Ignored || !webhookEvent.BookingId.HasValue)
                return false;

            if (webhookEvent.EventType == StripeWebhookEventType.CheckoutCompleted && webhookEvent.IsPaid)
            {
                var paidAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
                await _repository.TryCompletePaymentFromWebhookAsync(
                    webhookEvent.BookingId.Value,
                    PaymentStatus.Paid,
                    paidAtUtc,
                    webhookEvent.SessionId,
                    webhookEvent.PaymentIntentId,
                    cancellationToken);
                return true;
            }

            if (webhookEvent.EventType == StripeWebhookEventType.PaymentFailed)
            {
                await _repository.TryCompletePaymentFromWebhookAsync(
                    webhookEvent.BookingId.Value,
                    PaymentStatus.Failed,
                    null,
                    webhookEvent.SessionId,
                    webhookEvent.PaymentIntentId,
                    cancellationToken);
                return true;
            }

            return false;
        }

        private static MockPaymentResponse SpecifyUtc(MockPaymentResponse response) =>
            response with
            {
                ScheduledStartUtc = DateTime.SpecifyKind(response.ScheduledStartUtc, DateTimeKind.Utc),
                PaidAtUtc = response.PaidAtUtc.HasValue
                    ? DateTime.SpecifyKind(response.PaidAtUtc.Value, DateTimeKind.Utc)
                    : null
            };
    }
}
