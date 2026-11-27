using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Queries
{
    public sealed class GetTouristGuideBookingConfirmationQueryHandler : IRequestHandler<GetTouristGuideBookingConfirmationQuery, GuideBookingConfirmationResponse?>
    {
        private readonly IGuideBookingRepository _repository;
        public GetTouristGuideBookingConfirmationQueryHandler(IGuideBookingRepository repository) => _repository = repository;
        public Task<GuideBookingConfirmationResponse?> Handle(GetTouristGuideBookingConfirmationQuery request, CancellationToken cancellationToken) =>
            _repository.GetTouristConfirmationAsync(request.UserId, request.BookingId, cancellationToken);
    }
}
