using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Features.HostProfile.Requests.Commands;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscoverEgypt.Application.Features.HostProfile.Handlers.Commands
{
    public sealed class SaveHostProfileCommandHandler : IRequestHandler<SaveHostProfileCommand, Unit>
    {
        private readonly IHostProfileRepository _repository;
        private readonly IExperienceMediaStorage _mediaStorage;
        private readonly ILogger<SaveHostProfileCommandHandler> _logger;

        public SaveHostProfileCommandHandler(
            IHostProfileRepository repository,
            IExperienceMediaStorage mediaStorage,
            ILogger<SaveHostProfileCommandHandler> logger)
        {
            _repository = repository;
            _mediaStorage = mediaStorage;
            _logger = logger;
        }

        public async Task<Unit> Handle(SaveHostProfileCommand request, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            ArgumentNullException.ThrowIfNull(request.Profile);

            string? newImageKey = null;
            if (request.ImageContent is not null)
            {
                if (!await HasExpectedImageSignatureAsync(request.ImageContent, request.ImageExtension!, cancellationToken))
                    throw new ValidationException("The uploaded file contents do not match the selected image type.");

                newImageKey = $"host-profiles/{Guid.NewGuid():N}{request.ImageExtension}";
                await _mediaStorage.UploadAsync(newImageKey, request.ImageContentType!, request.ImageContent, cancellationToken);
            }

            string? previousImageKey;
            try
            {
                previousImageKey = await _repository.SaveAsync(
                    request.UserId,
                    request.Profile.HostType,
                    request.Profile.Bio,
                    newImageKey,
                    cancellationToken);
            }
            catch
            {
                if (newImageKey is not null)
                    await _mediaStorage.DeleteAsync(newImageKey, CancellationToken.None);
                throw;
            }

            if (previousImageKey is not null)
            {
                try
                {
                    await _mediaStorage.DeleteAsync(previousImageKey, cancellationToken);
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    _logger.LogWarning(exception, "Could not remove the replaced host profile image {StorageKey}.", previousImageKey);
                }
            }
            return Unit.Value;
        }

        private static async Task<bool> HasExpectedImageSignatureAsync(Stream content, string extension, CancellationToken cancellationToken)
        {
            if (!content.CanSeek) return false;
            var originalPosition = content.Position;
            try
            {
                content.Position = 0;
                var header = new byte[12];
                var count = 0;
                while (count < header.Length)
                {
                    var bytesRead = await content.ReadAsync(header.AsMemory(count), cancellationToken);
                    if (bytesRead == 0) break;
                    count += bytesRead;
                }
                return extension switch
                {
                    ".jpg" or ".jpeg" => count >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
                    ".png" => count >= 8 && header.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
                    ".webp" => count >= 12 && header.AsSpan(0, 4).SequenceEqual("RIFF"u8) && header.AsSpan(8, 4).SequenceEqual("WEBP"u8),
                    _ => false
                };
            }
            finally
            {
                content.Position = originalPosition;
            }
        }
    }
}
