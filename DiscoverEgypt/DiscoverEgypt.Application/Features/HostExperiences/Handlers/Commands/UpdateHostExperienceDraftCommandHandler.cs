using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class UpdateHostExperienceDraftCommandHandler(IHostExperienceRepository repository)
    : IRequestHandler<UpdateHostExperienceDraftCommand, bool>
{
    public Task<bool> Handle(UpdateHostExperienceDraftCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        ArgumentNullException.ThrowIfNull(request.Draft);

        return repository.UpdateDraftAsync(request.UserId, request.ExperienceId, request.Draft, cancellationToken);
    }
}
