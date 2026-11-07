using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Domain.Enums.Experiences;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands
{
    public sealed class DeleteHostExperienceMediaCommandHandler : IRequestHandler<DeleteHostExperienceMediaCommand, ExperienceMediaMutationOutcome>
    {
        private readonly IHostExperienceMediaRepository _repository;
        private readonly IExperienceMediaStorage _storage;
        private readonly ILogger<DeleteHostExperienceMediaCommandHandler> _logger;

        public DeleteHostExperienceMediaCommandHandler(
            IHostExperienceMediaRepository repository,
            IExperienceMediaStorage storage,
            ILogger<DeleteHostExperienceMediaCommandHandler> logger)
        {
            _repository = repository;
            _storage = storage;
            _logger = logger;
        }

        public async Task<ExperienceMediaMutationOutcome> Handle(
            DeleteHostExperienceMediaCommand request,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            string? storageKey = null;
            var result = await _repository.ExecuteMutationAsync(
                request.UserId,
                request.ExperienceId,
                (context, _) =>
                {
                    var experience = context.Experience;
                    if (!HostExperienceDraftPolicy.IsEditable(experience.Status))
                    {
                        return Task.FromResult(ExperienceMediaMutationOutcome.ExperienceUnavailable);
                    }

                    var media = experience.Media.SingleOrDefault(item => item.Id == request.MediaId);
                    if (media is null)
                    {
                        return Task.FromResult(ExperienceMediaMutationOutcome.MediaUnavailable);
                    }

                    storageKey = media.StorageKey;
                    var removedCover = media.IsCover;
                    experience.Media.Remove(media);
                    context.RemoveMedia(media);
                    if (removedCover)
                    {
                        var replacement = experience.Media
                            .Where(item => item.MediaType == MediaType.Image)
                            .OrderBy(item => item.DisplayOrder)
                            .ThenBy(item => item.Id)
                            .FirstOrDefault();
                        if (replacement is not null)
                        {
                            replacement.IsCover = true;
                        }
                    }

                    return Task.FromResult(ExperienceMediaMutationOutcome.Success);
                },
                cancellationToken);

            if (result == ExperienceMediaMutationOutcome.Success && storageKey is not null)
            {
                try
                {
                    await _storage.DeleteAsync(storageKey, cancellationToken);
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    _logger.LogError(exception, "Could not delete private experience media object {StorageKey}.", storageKey);
                }
            }

            return result;
        }
    }
}
