namespace DiscoverEgypt.Application.Contracts.Payments
{
    public sealed record StripeCheckoutSessionStatus(
        string SessionId,
        string? PaymentIntentId,
        int BookingId,
        string UserId,
        bool IsPaid,
        bool IsExpiredOrFailed);
}
