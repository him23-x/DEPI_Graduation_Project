using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries;

public sealed class GetHostExperienceMediaFileQueryHandler(
    IHostExperienceMediaRepository repository,
    IExperienceMediaStorage storage) : IRequestHandler<GetHostExperienceMediaFileQuery, ExperienceMediaDownload?>
{
    public async Task<ExperienceMediaDownload?> Handle(GetHostExperienceMediaFileQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        var media = await repository.GetOwnedMediaAsync(request.UserId, request.MediaId, cancellationToken);
        return media is null ? null : await storage.OpenReadAsync(media.StorageKey, cancellationToken);
    }
}
