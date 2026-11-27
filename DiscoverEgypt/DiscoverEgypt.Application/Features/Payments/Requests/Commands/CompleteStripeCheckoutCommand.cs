using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Payments.Requests.Commands
{
    public sealed record CompleteStripeCheckoutCommand(
        string UserId,
        int BookingId,
        string SessionId)
        : IRequest<MockPaymentResponse>;
}
