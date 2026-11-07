using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries
{
    public sealed class GetHostExperienceEvidenceFileForReviewQueryHandler
        : IRequestHandler<GetHostExperienceEvidenceFileForReviewQuery, ExperienceEvidenceDownload?>
    {
        private readonly IHostExperienceEvidenceRepository _repository;
        private readonly IExperienceEvidenceStorage _storage;

        public GetHostExperienceEvidenceFileForReviewQueryHandler(
            IHostExperienceEvidenceRepository repository,
            IExperienceEvidenceStorage storage)
        {
            _repository = repository;
            _storage = storage;
        }

        public async Task<ExperienceEvidenceDownload?> Handle(
            GetHostExperienceEvidenceFileForReviewQuery request,
            CancellationToken cancellationToken)
        {
            if (request.EvidenceId <= 0)
                return null;

            var file = await _repository.GetFileForReviewAsync(request.EvidenceId, cancellationToken);
            return file is null
                ? null
                : await _storage.OpenReadAsync(file.StorageKey, cancellationToken);
        }
    }
}

