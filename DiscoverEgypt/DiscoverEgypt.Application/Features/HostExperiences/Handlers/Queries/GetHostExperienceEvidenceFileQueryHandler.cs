using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.ViewModels.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries
{
    public sealed class GetHostExperienceEvidenceFileQueryHandler
        : IRequestHandler<GetHostExperienceEvidenceFileQuery, ExperienceEvidenceDownload?>
    {
        private readonly IHostExperienceEvidenceRepository _repository;
        private readonly IExperienceEvidenceStorage _storage;

        public GetHostExperienceEvidenceFileQueryHandler(
            IHostExperienceEvidenceRepository repository,
            IExperienceEvidenceStorage storage)
        {
            _repository = repository;
            _storage = storage;
        }

        public async Task<ExperienceEvidenceDownload?> Handle(GetHostExperienceEvidenceFileQuery request, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var file = await _repository.GetOwnedFileAsync(request.UserId, request.EvidenceId, cancellationToken);
            return file is null ? null : await _storage.OpenReadAsync(file.StorageKey, cancellationToken);
        }
    }
}

