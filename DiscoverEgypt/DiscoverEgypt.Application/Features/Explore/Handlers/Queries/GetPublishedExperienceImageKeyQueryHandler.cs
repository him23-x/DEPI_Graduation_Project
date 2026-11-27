using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.Explore.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.Explore.Handlers.Queries
{
    public sealed class GetPublishedExperienceImageKeyQueryHandler : IRequestHandler<GetPublishedExperienceImageKeyQuery, string?>
    {
        private readonly IExperienceExploreRepository _repository;

        public GetPublishedExperienceImageKeyQueryHandler(IExperienceExploreRepository repository)
        {
            _repository = repository;
        }

        public Task<string?> Handle(GetPublishedExperienceImageKeyQuery request, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(request.ExperienceId);
            return _repository.GetPublishedImageKeyAsync(request.ExperienceId, cancellationToken);
        }
    }
}
