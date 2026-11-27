namespace DiscoverEgypt.Application.Contracts.Payments
{
    public interface IStripePaymentGateway
    {
        Task<StripeCheckoutSessionCreated> CreateCheckoutSessionAsync(
            StripeCheckoutSessionRequest request,
            CancellationToken cancellationToken = default);

        Task<StripeCheckoutSessionStatus> GetCheckoutSessionAsync(
            string sessionId,
            CancellationToken cancellationToken = default);

        StripeWebhookEventResult ParseWebhookEvent(string payload, string signatureHeader);
    }
}
