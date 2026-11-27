using DiscoverEgypt.Application.Enums.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Commands
{
    public sealed record CancelTouristBookingCommand(string UserId, int BookingId) : IRequest<BookingCancellationOutcome>;
}
