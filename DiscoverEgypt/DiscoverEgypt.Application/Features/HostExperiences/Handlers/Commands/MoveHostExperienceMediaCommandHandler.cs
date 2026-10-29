using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using MediatR;

using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class MoveHostExperienceMediaCommandHandler(IHostExperienceMediaRepository repository)
    : IRequestHandler<MoveHostExperienceMediaCommand, ExperienceMediaMutationOutcome>
{
    public Task<ExperienceMediaMutationOutcome> Handle(MoveHostExperienceMediaCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return request.Direction is -1 or 1
            ? repository.MoveAsync(request.UserId, request.ExperienceId, request.MediaId, request.Direction, cancellationToken)
            : Task.FromResult(ExperienceMediaMutationOutcome.InvalidOrder);
    }
}
