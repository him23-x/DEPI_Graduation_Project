using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Features.Bookings.Services;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Queries
{
    public sealed class GetGuideBookingPageQueryHandler : IRequestHandler<GetGuideBookingPageQuery, GuideBookingFormResponse?>
    {
        private readonly GuideBookingService _service;
        public GetGuideBookingPageQueryHandler(GuideBookingService service) => _service = service;
        public Task<GuideBookingFormResponse?> Handle(GetGuideBookingPageQuery request, CancellationToken cancellationToken) =>
            _service.GetFormAsync(request.GuideProfileId, cancellationToken);
    }
}
