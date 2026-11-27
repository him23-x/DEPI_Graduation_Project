using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Queries
{
    public sealed record GetTouristGuideBookingConfirmationQuery(string UserId, int BookingId) : IRequest<GuideBookingConfirmationResponse?>;
}
