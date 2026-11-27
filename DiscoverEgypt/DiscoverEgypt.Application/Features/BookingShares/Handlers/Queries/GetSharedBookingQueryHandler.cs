using DiscoverEgypt.Application.Features.BookingShares.Requests.Queries;
using DiscoverEgypt.Application.Features.BookingShares.Services;
using DiscoverEgypt.Application.Responses.BookingShares;
using MediatR;

namespace DiscoverEgypt.Application.Features.BookingShares.Handlers.Queries
{
    public sealed class GetSharedBookingQueryHandler : IRequestHandler<GetSharedBookingQuery, SharedBookingResponse?>
    {
        private readonly BookingShareService _service;

        public GetSharedBookingQueryHandler(BookingShareService service) => _service = service;

        public Task<SharedBookingResponse?> Handle(GetSharedBookingQuery request, CancellationToken cancellationToken) =>
            _service.GetSharedAsync(request.Token, cancellationToken);
    }
}
