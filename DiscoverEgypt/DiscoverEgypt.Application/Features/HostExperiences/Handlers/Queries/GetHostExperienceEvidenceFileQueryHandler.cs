using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries;

public sealed class GetHostExperienceEvidenceFileQueryHandler(
    IHostExperienceEvidenceRepository repository,
    IExperienceEvidenceStorage storage) : IRequestHandler<GetHostExperienceEvidenceFileQuery, ExperienceEvidenceDownload?>
{
    public async Task<ExperienceEvidenceDownload?> Handle(GetHostExperienceEvidenceFileQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        var file = await repository.GetOwnedFileAsync(request.UserId, request.EvidenceId, cancellationToken);
        return file is null ? null : await storage.OpenReadAsync(file.StorageKey, cancellationToken);
    }
}
