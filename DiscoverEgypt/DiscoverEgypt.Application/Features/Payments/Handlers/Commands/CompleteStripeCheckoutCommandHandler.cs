using DiscoverEgypt.Application.Features.Payments.Requests.Commands;
using DiscoverEgypt.Application.Features.Payments.Services;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Payments.Handlers.Commands
{
    public sealed class CompleteStripeCheckoutCommandHandler
        : IRequestHandler<CompleteStripeCheckoutCommand, MockPaymentResponse>
    {
        private readonly MockPaymentService _service;

        public CompleteStripeCheckoutCommandHandler(MockPaymentService service) => _service = service;

        public Task<MockPaymentResponse> Handle(
            CompleteStripeCheckoutCommand request,
            CancellationToken cancellationToken) =>
            _service.CompleteCheckoutAsync(
                request.UserId,
                request.BookingId,
                request.SessionId,
                cancellationToken);
    }
}
