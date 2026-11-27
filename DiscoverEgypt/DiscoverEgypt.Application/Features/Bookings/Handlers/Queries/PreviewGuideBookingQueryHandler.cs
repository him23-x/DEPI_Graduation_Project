using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Features.Bookings.Services;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Queries
{
    public sealed class PreviewGuideBookingQueryHandler : IRequestHandler<PreviewGuideBookingQuery, GuideBookingReviewResponse>
    {
        private readonly GuideBookingService _service;
        public PreviewGuideBookingQueryHandler(GuideBookingService service) => _service = service;
        public Task<GuideBookingReviewResponse> Handle(PreviewGuideBookingQuery request, CancellationToken cancellationToken) =>
            _service.PreviewAsync(request.Form, cancellationToken);
    }
}
