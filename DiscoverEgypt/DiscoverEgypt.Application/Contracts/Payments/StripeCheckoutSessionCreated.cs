namespace DiscoverEgypt.Application.Contracts.Payments
{
    public sealed record StripeCheckoutSessionCreated(
        string SessionId,
        string CheckoutUrl);
}
