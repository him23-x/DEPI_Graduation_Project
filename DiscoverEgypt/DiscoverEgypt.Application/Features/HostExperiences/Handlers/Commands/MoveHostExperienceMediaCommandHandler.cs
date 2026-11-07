using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands
{
    public sealed class MoveHostExperienceMediaCommandHandler : IRequestHandler<MoveHostExperienceMediaCommand, ExperienceMediaMutationOutcome>
    {
        private readonly IHostExperienceMediaRepository _repository;

        public MoveHostExperienceMediaCommandHandler(IHostExperienceMediaRepository repository)
        {
            _repository = repository;
        }

        public Task<ExperienceMediaMutationOutcome> Handle(
            MoveHostExperienceMediaCommand request,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            if (request.Direction is not (-1 or 1))
            {
                return Task.FromResult(ExperienceMediaMutationOutcome.InvalidOrder);
            }

            return _repository.ExecuteMutationAsync(
                request.UserId,
                request.ExperienceId,
                async (context, token) =>
                {
                    var experience = context.Experience;
                    if (!HostExperienceDraftPolicy.IsEditable(experience.Status))
                    {
                        return ExperienceMediaMutationOutcome.ExperienceUnavailable;
                    }

                    var ordered = experience.Media
                        .OrderBy(item => item.DisplayOrder)
                        .ThenBy(item => item.Id)
                        .ToList();
                    var currentIndex = ordered.FindIndex(item => item.Id == request.MediaId);
                    if (currentIndex < 0)
                    {
                        return ExperienceMediaMutationOutcome.MediaUnavailable;
                    }

                    var neighborIndex = currentIndex + request.Direction;
                    if (neighborIndex < 0 || neighborIndex >= ordered.Count)
                    {
                        return ExperienceMediaMutationOutcome.InvalidOrder;
                    }

                    var current = ordered[currentIndex];
                    var neighbor = ordered[neighborIndex];
                    var temporaryOrder = ordered.Max(item => item.DisplayOrder) + 1;
                    var currentOrder = current.DisplayOrder;
                    var neighborOrder = neighbor.DisplayOrder;
                    current.DisplayOrder = temporaryOrder;
                    await context.SaveChangesAsync(token);
                    neighbor.DisplayOrder = currentOrder;
                    await context.SaveChangesAsync(token);
                    current.DisplayOrder = neighborOrder;

                    return ExperienceMediaMutationOutcome.Success;
                },
                cancellationToken);
        }
    }
}
