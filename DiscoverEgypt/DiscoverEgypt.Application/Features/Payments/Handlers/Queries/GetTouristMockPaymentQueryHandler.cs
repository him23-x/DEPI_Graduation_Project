using DiscoverEgypt.Application.Features.Payments.Requests.Queries;
using DiscoverEgypt.Application.Features.Payments.Services;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Payments.Handlers.Queries
{
    public sealed class GetTouristMockPaymentQueryHandler : IRequestHandler<GetTouristMockPaymentQuery, MockPaymentResponse?>
    {
        private readonly MockPaymentService _service;
        public GetTouristMockPaymentQueryHandler(MockPaymentService service) => _service = service;
        public Task<MockPaymentResponse?> Handle(GetTouristMockPaymentQuery request, CancellationToken cancellationToken) =>
            _service.GetAsync(request.UserId, request.BookingId, cancellationToken);
    }
}
