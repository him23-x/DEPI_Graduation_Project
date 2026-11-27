using DiscoverEgypt.Application.Responses.Disputes;
using MediatR;

namespace DiscoverEgypt.Application.Features.Disputes.Requests.Queries
{
    public sealed record GetTouristDisputeBookingQuery(string UserId, int BookingId)
        : IRequest<TouristDisputeBookingResponse?>;
}
