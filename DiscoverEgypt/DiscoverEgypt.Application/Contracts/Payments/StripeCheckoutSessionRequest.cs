namespace DiscoverEgypt.Application.Contracts.Payments
{
    public sealed record StripeCheckoutSessionRequest(
        int BookingId,
        string UserId,
        string ServiceTitle,
        decimal Amount,
        string SuccessUrl,
        string CancelUrl);
}
