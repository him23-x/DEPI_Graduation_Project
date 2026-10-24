using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries;

public sealed class GetHostExperienceMediaEditorQueryHandler(IHostExperienceMediaRepository repository)
    : IRequestHandler<GetHostExperienceMediaEditorQuery, HostExperienceMediaEditor?>
{
    public Task<HostExperienceMediaEditor?> Handle(GetHostExperienceMediaEditorQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.GetDraftEditorAsync(request.UserId, request.ExperienceId, cancellationToken);
    }
}
