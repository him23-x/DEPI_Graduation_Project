using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using DiscoverEgypt.Application.Features.Disputes.Services;
using DiscoverEgypt.Application.Responses.Disputes;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.Disputes.Handlers.Queries
{
    public sealed class GetTouristDisputeDetailsQueryHandler
        : IRequestHandler<GetTouristDisputeDetailsQuery, TouristDisputeDetailsResponse?>
    {
        private readonly TouristDisputeService _service;
        private readonly IValidator<GetTouristDisputeDetailsQuery> _validator;

        public GetTouristDisputeDetailsQueryHandler(
            TouristDisputeService service,
            IValidator<GetTouristDisputeDetailsQuery> validator)
        {
            _service = service;
            _validator = validator;
        }

        public async Task<TouristDisputeDetailsResponse?> Handle(
            GetTouristDisputeDetailsQuery request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            return await _service.GetDetailsAsync(request.UserId, request.DisputeId, cancellationToken);
        }
    }
}
