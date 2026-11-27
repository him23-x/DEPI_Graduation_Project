using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Payments.Requests.Queries
{
    public sealed record GetTouristMockPaymentQuery(string UserId, int BookingId) : IRequest<MockPaymentResponse?>;
}
