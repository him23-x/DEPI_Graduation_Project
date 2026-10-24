using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class MoveHostExperienceMediaCommandHandler(IHostExperienceMediaRepository repository)
    : IRequestHandler<MoveHostExperienceMediaCommand, ExperienceMediaMutationResult>
{
    public Task<ExperienceMediaMutationResult> Handle(MoveHostExperienceMediaCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return request.Direction is -1 or 1
            ? repository.MoveAsync(request.UserId, request.ExperienceId, request.MediaId, request.Direction, cancellationToken)
            : Task.FromResult(ExperienceMediaMutationResult.InvalidOrder);
    }
}
