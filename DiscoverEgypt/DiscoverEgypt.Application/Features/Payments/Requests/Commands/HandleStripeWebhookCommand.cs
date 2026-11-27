using MediatR;

namespace DiscoverEgypt.Application.Features.Payments.Requests.Commands
{
    public sealed record HandleStripeWebhookCommand(
        string Payload,
        string SignatureHeader)
        : IRequest<bool>;
}
