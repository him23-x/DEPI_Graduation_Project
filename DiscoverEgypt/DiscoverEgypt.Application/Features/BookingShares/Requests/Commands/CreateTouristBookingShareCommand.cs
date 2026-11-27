using DiscoverEgypt.Application.Responses.BookingShares;
using MediatR;

namespace DiscoverEgypt.Application.Features.BookingShares.Requests.Commands
{
    public sealed record CreateTouristBookingShareCommand(string UserId, int BookingId)
        : IRequest<BookingShareCreateResponse>;
}
