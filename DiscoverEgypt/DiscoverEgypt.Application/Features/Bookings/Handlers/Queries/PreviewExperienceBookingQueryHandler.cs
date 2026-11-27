using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Features.Bookings.Services;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Queries
{
    public sealed class PreviewExperienceBookingQueryHandler
        : IRequestHandler<PreviewExperienceBookingQuery, ExperienceBookingReviewResponse>
    {
        private readonly ExperienceBookingService _service;

        public PreviewExperienceBookingQueryHandler(ExperienceBookingService service) => _service = service;

        public Task<ExperienceBookingReviewResponse> Handle(PreviewExperienceBookingQuery request, CancellationToken cancellationToken) =>
            _service.PreviewAsync(request.Form, cancellationToken);
    }
}
