using DiscoverEgypt.Application.Features.Payments.Requests.Commands;
using DiscoverEgypt.Application.Features.Payments.Services;
using MediatR;

namespace DiscoverEgypt.Application.Features.Payments.Handlers.Commands
{
    public sealed class HandleStripeWebhookCommandHandler
        : IRequestHandler<HandleStripeWebhookCommand, bool>
    {
        private readonly MockPaymentService _service;

        public HandleStripeWebhookCommandHandler(MockPaymentService service) => _service = service;

        public Task<bool> Handle(HandleStripeWebhookCommand request, CancellationToken cancellationToken) =>
            _service.ProcessWebhookAsync(request.Payload, request.SignatureHeader, cancellationToken);
    }
}
