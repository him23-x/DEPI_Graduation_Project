using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Queries
{
    public sealed record GetExperienceBookingPageQuery(int ExperienceId) : IRequest<ExperienceBookingFormResponse?>;
}
