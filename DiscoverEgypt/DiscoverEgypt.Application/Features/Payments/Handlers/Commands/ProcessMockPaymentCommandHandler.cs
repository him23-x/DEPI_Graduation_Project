using DiscoverEgypt.Application.Features.Payments.Requests.Commands;
using DiscoverEgypt.Application.Features.Payments.Services;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Payments.Handlers.Commands
{
    public sealed class ProcessMockPaymentCommandHandler : IRequestHandler<ProcessMockPaymentCommand, MockPaymentResponse>
    {
        private readonly MockPaymentService _service;
        public ProcessMockPaymentCommandHandler(MockPaymentService service) => _service = service;
        public Task<MockPaymentResponse> Handle(ProcessMockPaymentCommand request, CancellationToken cancellationToken) =>
            _service.ProcessAsync(request.UserId, request.BookingId, request.Outcome, cancellationToken);
    }
}
