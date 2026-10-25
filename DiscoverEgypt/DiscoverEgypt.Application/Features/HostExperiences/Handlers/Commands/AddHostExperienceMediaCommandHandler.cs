using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class AddHostExperienceMediaCommandHandler(
    IHostExperienceMediaRepository repository,
    IExperienceMediaStorage storage,
    ILogger<AddHostExperienceMediaCommandHandler> logger)
    : IRequestHandler<AddHostExperienceMediaCommand, ExperienceMediaMutationResult>
{
    public async Task<ExperienceMediaMutationResult> Handle(AddHostExperienceMediaCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        if (request.Length <= 0 || request.Content is null || !request.Content.CanSeek)
        {
            return ExperienceMediaMutationResult.InvalidFile;
        }

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var (mediaType, expectedContentType, maxLength) = MediaFilePolicy.GetFileInfo(request.FileName);
        if (string.IsNullOrEmpty(expectedContentType)
            || !string.Equals(request.ContentType, expectedContentType, StringComparison.OrdinalIgnoreCase)
            || request.Length > maxLength
            || !await MediaFilePolicy.HasExpectedSignatureAsync(request.Content, extension, cancellationToken))
        {
            return ExperienceMediaMutationResult.InvalidFile;
        }

        var editor = await repository.GetDraftEditorAsync(request.UserId, request.ExperienceId, cancellationToken);
        if (editor is null)
        {
            return ExperienceMediaMutationResult.ExperienceUnavailable;
        }

        var mediaCount = editor.Media.Count(item => item.MediaType == mediaType);
        if (mediaType == MediaType.Image && mediaCount >= MediaFilePolicy.MaxImages)
        {
            return ExperienceMediaMutationResult.ImageLimitReached;
        }
        if (mediaType == MediaType.Video && mediaCount >= MediaFilePolicy.MaxVideos)
        {
            return ExperienceMediaMutationResult.VideoLimitReached;
        }

        var storageKey = $"experiences/{request.ExperienceId}/{Guid.NewGuid():N}{extension}";
        request.Content.Position = 0;
        await storage.UploadAsync(storageKey, expectedContentType, request.Content, cancellationToken);

        try
        {
            var result = await repository.AddAsync(request.UserId, request.ExperienceId, mediaType, storageKey, cancellationToken);
            if (result != ExperienceMediaMutationResult.Success)
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
            await storage.DeleteAsync(storageKey, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Could not clean up private experience media object {StorageKey}.", storageKey);
        }
    }
}
