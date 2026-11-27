using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Guides;
using DiscoverEgypt.Application.Features.ApprovedGuides.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.ApprovedGuides.Handlers.Queries
{
    public sealed class GetApprovedGuideDetailsQueryHandler : IRequestHandler<GetApprovedGuideDetailsQuery, GuideProfileDetailsDto?>
    {
        private readonly IApprovedGuideRepository _repository;

        public GetApprovedGuideDetailsQueryHandler(IApprovedGuideRepository repository) => _repository = repository;

        public Task<GuideProfileDetailsDto?> Handle(GetApprovedGuideDetailsQuery request, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(request.GuideProfileId);
            return _repository.GetDetailsAsync(request.GuideProfileId, cancellationToken);
        }
    }
}
