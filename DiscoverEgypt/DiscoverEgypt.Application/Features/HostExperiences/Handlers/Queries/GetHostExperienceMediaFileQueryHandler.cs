using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.ViewModels.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries
{
    public sealed class GetHostExperienceMediaFileQueryHandler
        : IRequestHandler<GetHostExperienceMediaFileQuery, ExperienceMediaDownload?>
    {
        private readonly IHostExperienceMediaRepository _repository;
        private readonly IExperienceMediaStorage _storage;

        public GetHostExperienceMediaFileQueryHandler(
            IHostExperienceMediaRepository repository,
            IExperienceMediaStorage storage)
        {
            _repository = repository;
            _storage = storage;
        }

        public async Task<ExperienceMediaDownload?> Handle(GetHostExperienceMediaFileQuery request, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var media = await _repository.GetOwnedMediaAsync(request.UserId, request.MediaId, cancellationToken);
            return media is null ? null : await _storage.OpenReadAsync(media.StorageKey, cancellationToken);
        }
    }
}

