using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using DiscoverEgypt.Application.Features.Disputes.Services;
using DiscoverEgypt.Application.Responses.Disputes;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.Disputes.Handlers.Queries
{
    public sealed class GetTouristDisputesQueryHandler
        : IRequestHandler<GetTouristDisputesQuery, IReadOnlyList<TouristDisputeListItemResponse>>
    {
        private readonly TouristDisputeService _service;
        private readonly IValidator<GetTouristDisputesQuery> _validator;

        public GetTouristDisputesQueryHandler(
            TouristDisputeService service,
            IValidator<GetTouristDisputesQuery> validator)
        {
            _service = service;
            _validator = validator;
        }

        public async Task<IReadOnlyList<TouristDisputeListItemResponse>> Handle(
            GetTouristDisputesQuery request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            return await _service.GetForTouristAsync(request.UserId, cancellationToken);
        }
    }
}
