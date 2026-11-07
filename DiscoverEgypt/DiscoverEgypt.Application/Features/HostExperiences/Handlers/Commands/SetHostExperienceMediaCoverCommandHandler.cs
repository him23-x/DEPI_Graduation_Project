using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Domain.Enums.Experiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands
{
    public sealed class SetHostExperienceMediaCoverCommandHandler : IRequestHandler<SetHostExperienceMediaCoverCommand, ExperienceMediaMutationOutcome>
    {
        private readonly IHostExperienceMediaRepository _repository;

        public SetHostExperienceMediaCoverCommandHandler(IHostExperienceMediaRepository repository)
        {
            _repository = repository;
        }

        public Task<ExperienceMediaMutationOutcome> Handle(
            SetHostExperienceMediaCoverCommand request,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            return _repository.ExecuteMutationAsync(
                request.UserId,
                request.ExperienceId,
                (context, _) =>
                {
                    var experience = context.Experience;
                    if (!HostExperienceDraftPolicy.IsEditable(experience.Status))
                    {
                        return Task.FromResult(ExperienceMediaMutationOutcome.ExperienceUnavailable);
                    }

                    var selected = experience.Media.SingleOrDefault(item => item.Id == request.MediaId);
                    if (selected is null)
                    {
                        return Task.FromResult(ExperienceMediaMutationOutcome.MediaUnavailable);
                    }
                    if (selected.MediaType != MediaType.Image)
                    {
                        return Task.FromResult(ExperienceMediaMutationOutcome.CoverMustBeImage);
                    }

                    foreach (var image in experience.Media.Where(item => item.MediaType == MediaType.Image))
                    {
                        image.IsCover = image.Id == selected.Id;
                    }

                    return Task.FromResult(ExperienceMediaMutationOutcome.Success);
                },
                cancellationToken);
        }
    }
}
