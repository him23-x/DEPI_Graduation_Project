using DiscoverEgypt.Application.Enums.Payments;

namespace DiscoverEgypt.Application.Contracts.Payments
{
    public sealed record StripeWebhookEventResult(
        StripeWebhookEventType EventType,
        int? BookingId,
        string? UserId,
        string? SessionId,
        string? PaymentIntentId,
        bool IsPaid);
}
