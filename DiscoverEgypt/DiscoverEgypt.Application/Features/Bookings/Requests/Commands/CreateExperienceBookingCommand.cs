using DiscoverEgypt.Application.DTOs.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Commands
{
    public sealed record CreateExperienceBookingCommand(string UserId, ExperienceBookingInputDto Form) : IRequest<int>;
}
