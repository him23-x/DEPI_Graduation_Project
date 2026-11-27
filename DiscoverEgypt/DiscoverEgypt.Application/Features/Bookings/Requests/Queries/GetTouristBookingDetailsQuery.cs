using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Queries
{
    public sealed record GetTouristBookingDetailsQuery(string UserId, int BookingId) : IRequest<TouristBookingDetailsResponse?>;
}
