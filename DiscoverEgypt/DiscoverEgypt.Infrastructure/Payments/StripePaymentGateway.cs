using System.Globalization;
using DiscoverEgypt.Application.Contracts.Payments;
using DiscoverEgypt.Application.Enums.Payments;
using DiscoverEgypt.Application.Models;
using FluentValidation;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace DiscoverEgypt.Infrastructure.Payments
{
    public sealed class StripePaymentGateway : IStripePaymentGateway
    {
        private readonly StripeSettings _settings;

        public StripePaymentGateway(IOptions<StripeSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<StripeCheckoutSessionCreated> CreateCheckoutSessionAsync(
            StripeCheckoutSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            var client = CreateClientOrThrow();
            var unitAmount = ConvertToSmallestUnit(request.Amount);
            var currency = string.IsNullOrWhiteSpace(_settings.Currency) ? "egp" : _settings.Currency.Trim().ToLowerInvariant();
            var metadata = BuildMetadata(request.BookingId, request.UserId);

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl,
                ClientReferenceId = request.BookingId.ToString(CultureInfo.InvariantCulture),
                Metadata = metadata,
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = new Dictionary<string, string>(metadata)
                },
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency,
                            UnitAmount = unitAmount,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = string.IsNullOrWhiteSpace(request.ServiceTitle)
                                    ? $"Discover Egypt Booking #{request.BookingId}"
                                    : request.ServiceTitle
                            }
                        }
                    }
                ]
            };

            try
            {
                var sessionService = new SessionService(client);
                var session = await sessionService.CreateAsync(options, cancellationToken: cancellationToken);
                if (string.IsNullOrWhiteSpace(session.Id) || string.IsNullOrWhiteSpace(session.Url))
                    throw new ValidationException("Stripe did not return a valid checkout session URL.");

                return new StripeCheckoutSessionCreated(session.Id, session.Url);
            }
            catch (StripeException ex)
            {
                throw new ValidationException($"Stripe checkout error: {ex.StripeError?.Message ?? ex.Message}");
            }
        }

        public async Task<StripeCheckoutSessionStatus> GetCheckoutSessionAsync(
            string sessionId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
            var client = CreateClientOrThrow();

            try
            {
                var sessionService = new SessionService(client);
                var session = await sessionService.GetAsync(sessionId, cancellationToken: cancellationToken);

                var bookingId = ParseBookingId(session.Metadata, session.ClientReferenceId);
                var userId = ParseUserId(session.Metadata);
                var isPaid = string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase);
                var isExpiredOrFailed = string.Equals(session.Status, "expired", StringComparison.OrdinalIgnoreCase);

                return new StripeCheckoutSessionStatus(
                    session.Id,
                    session.PaymentIntentId,
                    bookingId,
                    userId,
                    isPaid,
                    isExpiredOrFailed);
            }
            catch (StripeException ex)
            {
                throw new ValidationException($"Could not verify Stripe checkout session: {ex.StripeError?.Message ?? ex.Message}");
            }
        }

        public StripeWebhookEventResult ParseWebhookEvent(string payload, string signatureHeader)
        {
            if (string.IsNullOrWhiteSpace(_settings.WebhookSecret))
                throw new ValidationException("Stripe webhook secret is not configured.");

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(payload, signatureHeader, _settings.WebhookSecret);
            }
            catch (StripeException ex)
            {
                throw new ValidationException($"Invalid Stripe webhook signature: {ex.Message}");
            }

            if (stripeEvent.Type is EventTypes.CheckoutSessionCompleted or EventTypes.CheckoutSessionAsyncPaymentSucceeded)
            {
                var session = stripeEvent.Data.Object as Session;
                var bookingId = ParseNullableBookingId(session?.Metadata, session?.ClientReferenceId);
                var userId = ParseUserId(session?.Metadata);
                var isPaid = string.Equals(session?.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase);
                return new StripeWebhookEventResult(
                    StripeWebhookEventType.CheckoutCompleted,
                    bookingId,
                    userId,
                    session?.Id,
                    session?.PaymentIntentId,
                    isPaid);
            }

            if (stripeEvent.Type is EventTypes.CheckoutSessionAsyncPaymentFailed or EventTypes.CheckoutSessionExpired)
            {
                var session = stripeEvent.Data.Object as Session;
                var bookingId = ParseNullableBookingId(session?.Metadata, session?.ClientReferenceId);
                var userId = ParseUserId(session?.Metadata);
                return new StripeWebhookEventResult(
                    StripeWebhookEventType.PaymentFailed,
                    bookingId,
                    userId,
                    session?.Id,
                    session?.PaymentIntentId,
                    false);
            }

            if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
            {
                var intent = stripeEvent.Data.Object as PaymentIntent;
                var bookingId = ParseNullableBookingId(intent?.Metadata, null);
                var userId = ParseUserId(intent?.Metadata);
                return new StripeWebhookEventResult(
                    StripeWebhookEventType.PaymentFailed,
                    bookingId,
                    userId,
                    null,
                    intent?.Id,
                    false);
            }

            return new StripeWebhookEventResult(StripeWebhookEventType.Ignored, null, null, null, null, false);
        }

        private StripeClient CreateClientOrThrow()
        {
            if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            {
                throw new ValidationException(
                    "Stripe test keys are not configured yet. Add Stripe:SecretKey (sk_test_...) in User Secrets or appsettings.Development.json to use Stripe Checkout.");
            }

            return new StripeClient(_settings.SecretKey);
        }

        private static long ConvertToSmallestUnit(decimal amount)
        {
            var unitAmount = (long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);
            if (unitAmount <= 0)
                throw new ValidationException("Booking payment amount must be greater than zero.");
            return unitAmount;
        }

        private static Dictionary<string, string> BuildMetadata(int bookingId, string userId) =>
            new()
            {
                ["booking_id"] = bookingId.ToString(CultureInfo.InvariantCulture),
                ["user_id"] = userId
            };

        private static int ParseBookingId(IDictionary<string, string>? metadata, string? clientReferenceId) =>
            ParseNullableBookingId(metadata, clientReferenceId) ?? 0;

        private static int? ParseNullableBookingId(IDictionary<string, string>? metadata, string? clientReferenceId)
        {
            if (metadata is not null
                && metadata.TryGetValue("booking_id", out var metadataValue)
                && int.TryParse(metadataValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedFromMetadata))
            {
                return parsedFromMetadata;
            }

            if (int.TryParse(clientReferenceId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedFromReference))
                return parsedFromReference;

            return null;
        }

        private static string ParseUserId(IDictionary<string, string>? metadata) =>
            metadata is not null && metadata.TryGetValue("user_id", out var userId)
                ? userId
                : string.Empty;
    }
}
