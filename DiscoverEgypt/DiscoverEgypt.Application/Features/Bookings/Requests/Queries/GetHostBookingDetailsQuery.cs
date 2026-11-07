using DiscoverEgypt.Application.ViewModels.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Queries
{
    public sealed record GetHostBookingDetailsQuery(string UserId, int BookingId)
        : IRequest<BookingDetailsViewModel?>;
}

