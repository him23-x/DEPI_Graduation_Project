using DiscoverEgypt.Application.Contracts.Payments;
using DiscoverEgypt.Application.Features.Payments.Requests.Commands;
using DiscoverEgypt.Application.Features.Payments.Services;
using MediatR;

namespace DiscoverEgypt.Application.Features.Payments.Handlers.Commands
{
    public sealed class CreateStripeCheckoutSessionCommandHandler
        : IRequestHandler<CreateStripeCheckoutSessionCommand, StripeCheckoutSessionCreated>
    {
        private readonly MockPaymentService _service;

        public CreateStripeCheckoutSessionCommandHandler(MockPaymentService service) => _service = service;

        public Task<StripeCheckoutSessionCreated> Handle(
            CreateStripeCheckoutSessionCommand request,
            CancellationToken cancellationToken) =>
            _service.CreateCheckoutSessionAsync(
                request.UserId,
                request.BookingId,
                request.SuccessUrl,
                request.CancelUrl,
                cancellationToken);
    }
}
