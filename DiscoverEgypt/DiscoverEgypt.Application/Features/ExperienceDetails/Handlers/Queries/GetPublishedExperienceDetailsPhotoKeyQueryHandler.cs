using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.ExperienceDetails.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.ExperienceDetails.Handlers.Queries
{
    public sealed class GetPublishedExperienceDetailsPhotoKeyQueryHandler : IRequestHandler<GetPublishedExperienceDetailsPhotoKeyQuery, string?>
    {
        private readonly IExperienceDetailsRepository _repository;

        public GetPublishedExperienceDetailsPhotoKeyQueryHandler(IExperienceDetailsRepository repository) => _repository = repository;

        public Task<string?> Handle(GetPublishedExperienceDetailsPhotoKeyQuery request, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(request.ExperienceId);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(request.MediaId);
            return _repository.GetPublishedImageKeyAsync(request.ExperienceId, request.MediaId, cancellationToken);
        }
    }
}
