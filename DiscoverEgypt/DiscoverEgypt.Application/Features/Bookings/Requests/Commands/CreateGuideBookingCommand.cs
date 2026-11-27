using DiscoverEgypt.Application.DTOs.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Commands
{
    public sealed record CreateGuideBookingCommand(string UserId, GuideBookingInputDto Form) : IRequest<int>;
}
