using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.ApprovedGuides.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.ApprovedGuides.Handlers.Queries
{
    public sealed class GetApprovedGuideImageKeyQueryHandler : IRequestHandler<GetApprovedGuideImageKeyQuery, string?>
    {
        private readonly IApprovedGuideRepository _repository;

        public GetApprovedGuideImageKeyQueryHandler(IApprovedGuideRepository repository) => _repository = repository;

        public Task<string?> Handle(GetApprovedGuideImageKeyQuery request, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(request.GuideProfileId);
            return _repository.GetApprovedProfileImageKeyAsync(request.GuideProfileId, cancellationToken);
        }
    }
}
