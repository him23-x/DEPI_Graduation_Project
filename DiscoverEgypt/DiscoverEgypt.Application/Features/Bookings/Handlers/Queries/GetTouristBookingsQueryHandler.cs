using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Features.Bookings.Services;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Queries
{
    public sealed class GetTouristBookingsQueryHandler : IRequestHandler<GetTouristBookingsQuery, IReadOnlyList<TouristBookingListItemResponse>>
    {
        private readonly TouristBookingService _service;
        public GetTouristBookingsQueryHandler(TouristBookingService service) => _service = service;
        public Task<IReadOnlyList<TouristBookingListItemResponse>> Handle(GetTouristBookingsQuery request, CancellationToken cancellationToken) =>
            _service.GetForTouristAsync(request.UserId, cancellationToken);
    }
}
