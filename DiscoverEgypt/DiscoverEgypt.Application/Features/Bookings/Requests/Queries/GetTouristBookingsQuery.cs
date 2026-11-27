using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Queries
{
    public sealed record GetTouristBookingsQuery(string UserId) : IRequest<IReadOnlyList<TouristBookingListItemResponse>>;
}
