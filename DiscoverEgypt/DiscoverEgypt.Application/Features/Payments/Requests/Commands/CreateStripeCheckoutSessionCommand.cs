using DiscoverEgypt.Application.Contracts.Payments;
using MediatR;

namespace DiscoverEgypt.Application.Features.Payments.Requests.Commands
{
    public sealed record CreateStripeCheckoutSessionCommand(
        string UserId,
        int BookingId,
        string SuccessUrl,
        string CancelUrl)
        : IRequest<StripeCheckoutSessionCreated>;
}
