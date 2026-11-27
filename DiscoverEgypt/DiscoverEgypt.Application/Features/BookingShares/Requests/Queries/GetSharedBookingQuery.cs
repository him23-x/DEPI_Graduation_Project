using DiscoverEgypt.Application.Responses.BookingShares;
using MediatR;

namespace DiscoverEgypt.Application.Features.BookingShares.Requests.Queries
{
    public sealed record GetSharedBookingQuery(string Token) : IRequest<SharedBookingResponse?>;
}
