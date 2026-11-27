using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Queries
{
    public sealed class GetTouristExperienceBookingConfirmationQueryHandler
        : IRequestHandler<GetTouristExperienceBookingConfirmationQuery, ExperienceBookingConfirmationResponse?>
    {
        private readonly IExperienceBookingRepository _repository;

        public GetTouristExperienceBookingConfirmationQueryHandler(IExperienceBookingRepository repository) => _repository = repository;

        public Task<ExperienceBookingConfirmationResponse?> Handle(
            GetTouristExperienceBookingConfirmationQuery request,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            return _repository.GetTouristConfirmationAsync(request.UserId, request.BookingId, cancellationToken);
        }
    }
}
