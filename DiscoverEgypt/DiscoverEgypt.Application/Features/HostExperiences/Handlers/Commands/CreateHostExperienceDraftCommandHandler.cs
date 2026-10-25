using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class CreateHostExperienceDraftCommandHandler(IHostExperienceRepository repository)
    : IRequestHandler<CreateHostExperienceDraftCommand, int?>
{
    public Task<int?> Handle(CreateHostExperienceDraftCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        ArgumentNullException.ThrowIfNull(request.Draft);

        return repository.CreateDraftAsync(request.UserId, request.Draft, cancellationToken);
    }
}
