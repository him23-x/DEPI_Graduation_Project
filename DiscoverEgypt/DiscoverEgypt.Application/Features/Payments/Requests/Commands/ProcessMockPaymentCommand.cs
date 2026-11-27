using DiscoverEgypt.Application.Enums.Payments;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Payments.Requests.Commands
{
    public sealed record ProcessMockPaymentCommand(string UserId, int BookingId, PaymentSimulationOutcome Outcome)
        : IRequest<MockPaymentResponse>;
}
