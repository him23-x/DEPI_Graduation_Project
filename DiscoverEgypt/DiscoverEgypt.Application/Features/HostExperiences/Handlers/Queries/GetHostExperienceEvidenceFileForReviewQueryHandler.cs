using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries;

public sealed class GetHostExperienceEvidenceFileForReviewQueryHandler(
    IHostExperienceEvidenceRepository repository,
    IExperienceEvidenceStorage storage)
    : IRequestHandler<GetHostExperienceEvidenceFileForReviewQuery, ExperienceEvidenceDownload?>
{
    public async Task<ExperienceEvidenceDownload?> Handle(
        GetHostExperienceEvidenceFileForReviewQuery request,
        CancellationToken cancellationToken)
    {
        if (request.EvidenceId <= 0)
            return null;

        var file = await repository.GetFileForReviewAsync(request.EvidenceId, cancellationToken);
        return file is null
            ? null
            : await storage.OpenReadAsync(file.StorageKey, cancellationToken);
    }
}
