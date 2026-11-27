using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using DiscoverEgypt.Application.Features.Disputes.Services;
using DiscoverEgypt.Application.Responses.Disputes;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.Disputes.Handlers.Queries
{
    public sealed class GetTouristDisputeBookingQueryHandler
        : IRequestHandler<GetTouristDisputeBookingQuery, TouristDisputeBookingResponse?>
    {
        private readonly TouristDisputeService _service;
        private readonly IValidator<GetTouristDisputeBookingQuery> _validator;

        public GetTouristDisputeBookingQueryHandler(
            TouristDisputeService service,
            IValidator<GetTouristDisputeBookingQuery> validator)
        {
            _service = service;
            _validator = validator;
        }

        public async Task<TouristDisputeBookingResponse?> Handle(
            GetTouristDisputeBookingQuery request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            return await _service.GetEligibleBookingAsync(request.UserId, request.BookingId, cancellationToken);
        }
    }
}
