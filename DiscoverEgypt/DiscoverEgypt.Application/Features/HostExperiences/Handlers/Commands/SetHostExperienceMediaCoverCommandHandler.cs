using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class SetHostExperienceMediaCoverCommandHandler(IHostExperienceMediaRepository repository)
    : IRequestHandler<SetHostExperienceMediaCoverCommand, ExperienceMediaMutationResult>
{
    public Task<ExperienceMediaMutationResult> Handle(SetHostExperienceMediaCoverCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.SetCoverAsync(request.UserId, request.ExperienceId, request.MediaId, cancellationToken);
    }
}
