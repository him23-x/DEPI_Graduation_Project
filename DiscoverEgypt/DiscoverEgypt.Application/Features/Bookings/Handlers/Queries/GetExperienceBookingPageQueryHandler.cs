using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Features.Bookings.Services;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Queries
{
    public sealed class GetExperienceBookingPageQueryHandler
        : IRequestHandler<GetExperienceBookingPageQuery, ExperienceBookingFormResponse?>
    {
        private readonly ExperienceBookingService _service;

        public GetExperienceBookingPageQueryHandler(ExperienceBookingService service) => _service = service;

        public Task<ExperienceBookingFormResponse?> Handle(GetExperienceBookingPageQuery request, CancellationToken cancellationToken) =>
            _service.GetFormAsync(request.ExperienceId, cancellationToken);
    }
}
