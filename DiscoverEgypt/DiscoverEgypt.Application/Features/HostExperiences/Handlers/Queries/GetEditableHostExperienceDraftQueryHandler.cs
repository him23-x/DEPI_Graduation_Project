using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries;

public sealed class GetEditableHostExperienceDraftQueryHandler(IHostExperienceRepository repository)
    : IRequestHandler<GetEditableHostExperienceDraftQuery, HostExperienceDraftEditor?>
{
    public Task<HostExperienceDraftEditor?> Handle(GetEditableHostExperienceDraftQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.GetEditableDraftAsync(request.UserId, request.ExperienceId, cancellationToken);
    }
}
