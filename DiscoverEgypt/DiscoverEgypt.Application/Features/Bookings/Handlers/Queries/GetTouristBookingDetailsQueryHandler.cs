using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Features.Bookings.Services;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Queries
{
    public sealed class GetTouristBookingDetailsQueryHandler : IRequestHandler<GetTouristBookingDetailsQuery, TouristBookingDetailsResponse?>
    {
        private readonly TouristBookingService _service;
        public GetTouristBookingDetailsQueryHandler(TouristBookingService service) => _service = service;
        public Task<TouristBookingDetailsResponse?> Handle(GetTouristBookingDetailsQuery request, CancellationToken cancellationToken) =>
            _service.GetDetailsAsync(request.UserId, request.BookingId, cancellationToken);
    }
}
