using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.AdminDisputes.Requests.Queries;
using DiscoverEgypt.Application.Features.AdminDisputes.Validators;
using DiscoverEgypt.Application.Responses.Disputes;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminDisputes.Handlers.Queries
{
    public sealed class GetAdminDisputeDetailsQueryHandler
        : IRequestHandler<GetAdminDisputeDetailsQuery, AdminDisputeDetailsResponse?>
    {
        private readonly IAdminDisputeReviewRepository _repository;
        private readonly IValidator<GetAdminDisputeDetailsQuery> _validator;

        public GetAdminDisputeDetailsQueryHandler(
            IAdminDisputeReviewRepository repository,
            IValidator<GetAdminDisputeDetailsQuery> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<AdminDisputeDetailsResponse?> Handle(
            GetAdminDisputeDetailsQuery request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            return await _repository.GetOpenDetailsAsync(request.DisputeId, cancellationToken);
        }
    }
}
