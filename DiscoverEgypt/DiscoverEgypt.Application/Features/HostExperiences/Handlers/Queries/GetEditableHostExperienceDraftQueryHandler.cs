using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries;

public sealed class GetEditableHostExperienceDraftQueryHandler(IHostExperienceRepository repository)
    : IRequestHandler<GetEditableHostExperienceDraftQuery, HostExperienceDraftEditorDto?>
{
    public Task<HostExperienceDraftEditorDto?> Handle(GetEditableHostExperienceDraftQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.GetEditableDraftAsync(request.UserId, request.ExperienceId, cancellationToken);
    }
}
