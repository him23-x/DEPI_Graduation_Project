using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.AdminDisputes.Requests.Queries;
using DiscoverEgypt.Application.Responses.Disputes;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminDisputes.Handlers.Queries
{
    public sealed class GetAdminDisputeQueueQueryHandler
        : IRequestHandler<GetAdminDisputeQueueQuery, IReadOnlyList<AdminDisputeQueueItemResponse>>
    {
        private readonly IAdminDisputeReviewRepository _repository;

        public GetAdminDisputeQueueQueryHandler(IAdminDisputeReviewRepository repository) => _repository = repository;

        public Task<IReadOnlyList<AdminDisputeQueueItemResponse>> Handle(
            GetAdminDisputeQueueQuery request,
            CancellationToken cancellationToken) => _repository.GetOpenAsync(cancellationToken);
    }
}
