using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Experiences;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands
{
    public sealed class AddHostExperienceMediaCommandHandler : IRequestHandler<AddHostExperienceMediaCommand, ExperienceMediaMutationOutcome>
    {
        private readonly IHostExperienceMediaRepository _repository;
        private readonly IExperienceMediaStorage _storage;
        private readonly ILogger<AddHostExperienceMediaCommandHandler> _logger;

        public AddHostExperienceMediaCommandHandler(
            IHostExperienceMediaRepository repository,
            IExperienceMediaStorage storage,
            ILogger<AddHostExperienceMediaCommandHandler> logger)
        {
            _repository = repository;
            _storage = storage;
            _logger = logger;
        }

        public async Task<ExperienceMediaMutationOutcome> Handle(
            AddHostExperienceMediaCommand request,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            if (request.Length <= 0 || request.Content is null || !request.Content.CanSeek)
            {
                return ExperienceMediaMutationOutcome.InvalidFile;
            }

            var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
            var (mediaType, expectedContentType, maxLength) = MediaFilePolicy.GetFileInfo(request.FileName);
            if (string.IsNullOrEmpty(expectedContentType)
                || !string.Equals(request.ContentType, expectedContentType, StringComparison.OrdinalIgnoreCase)
                || request.Length > maxLength
                || !await MediaFilePolicy.HasExpectedSignatureAsync(request.Content, extension, cancellationToken))
            {
                return ExperienceMediaMutationOutcome.InvalidFile;
            }

            var editor = await _repository.GetDraftEditorAsync(request.UserId, request.ExperienceId, cancellationToken);
            if (editor is null)
            {
                return ExperienceMediaMutationOutcome.ExperienceUnavailable;
            }

            var mediaCount = editor.Media.Count(item => item.MediaType == mediaType);
            if (mediaType == MediaType.Image && mediaCount >= MediaFilePolicy.MaxImages)
            {
                return ExperienceMediaMutationOutcome.ImageLimitReached;
            }
            if (mediaType == MediaType.Video && mediaCount >= MediaFilePolicy.MaxVideos)
            {
                return ExperienceMediaMutationOutcome.VideoLimitReached;
            }

            var storageKey = $"experiences/{request.ExperienceId}/{Guid.NewGuid():N}{extension}";
            request.Content.Position = 0;
            await _storage.UploadAsync(storageKey, expectedContentType, request.Content, cancellationToken);

            try
            {
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

                        var currentCount = experience.Media.Count(item => item.MediaType == mediaType);
                        if (mediaType == MediaType.Image && currentCount >= MediaFilePolicy.MaxImages)
                        {
                            return Task.FromResult(ExperienceMediaMutationOutcome.ImageLimitReached);
                        }
                        if (mediaType == MediaType.Video && currentCount >= MediaFilePolicy.MaxVideos)
                        {
                            return Task.FromResult(ExperienceMediaMutationOutcome.VideoLimitReached);
                        }

                        experience.Media.Add(new ExperienceMedia
                        {
                            MediaType = mediaType,
                            StorageKey = storageKey,
                            DisplayOrder = experience.Media.Count == 0 ? 0 : experience.Media.Max(item => item.DisplayOrder) + 1,
                            IsCover = mediaType == MediaType.Image && !experience.Media.Any(item => item.IsCover),
                            CreatedAt = DateTime.UtcNow
                        });
                        return Task.FromResult(ExperienceMediaMutationOutcome.Success);
                    },
                    cancellationToken);

                if (result != ExperienceMediaMutationOutcome.Success)
                {
                    await TryDeleteStorageObjectAsync(storageKey, cancellationToken);
                }

                return result;
            }
            catch
            {
                await TryDeleteStorageObjectAsync(storageKey, cancellationToken);
                throw;
            }
        }

        private async Task TryDeleteStorageObjectAsync(string storageKey, CancellationToken cancellationToken)
        {
            try
            {
                await _storage.DeleteAsync(storageKey, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogError(exception, "Could not clean up private experience media object {StorageKey}.", storageKey);
            }
        }
    }
}
