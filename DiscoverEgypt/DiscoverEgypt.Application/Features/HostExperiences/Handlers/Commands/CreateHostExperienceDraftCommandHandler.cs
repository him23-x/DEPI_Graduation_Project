using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Application.Features.HostExperiences.Validators;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class CreateHostExperienceDraftCommandHandler(IHostExperienceRepository repository)
    : IRequestHandler<CreateHostExperienceDraftCommand, int?>
{
    public Task<int?> Handle(CreateHostExperienceDraftCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        ArgumentNullException.ThrowIfNull(request.Draft);

        if (!HostExperienceDraftRules.IsValid(request.Draft))
        {
            return Task.FromResult<int?>(null);
        }

        return repository.CreateDraftAsync(request.UserId, request.Draft, cancellationToken);
    }
}
