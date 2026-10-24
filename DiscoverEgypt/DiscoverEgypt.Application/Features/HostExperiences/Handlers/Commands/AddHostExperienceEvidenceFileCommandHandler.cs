using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class AddHostExperienceEvidenceFileCommandHandler(
    IHostExperienceEvidenceRepository repository,
    IExperienceEvidenceStorage storage,
    ILogger<AddHostExperienceEvidenceFileCommandHandler> logger)
    : IRequestHandler<AddHostExperienceEvidenceFileCommand, ExperienceEvidenceMutationResult>
{
    private const long MaxFileLength = 10 * 1024 * 1024;

    public async Task<ExperienceEvidenceMutationResult> Handle(AddHostExperienceEvidenceFileCommand request, CancellationToken cancellationToken)
    {
        if (request.Length <= 0 || request.Length > MaxFileLength || request.Content is null || !request.Content.CanSeek)
        {
            return ExperienceEvidenceMutationResult.InvalidFile;
        }

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var expectedType = extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => null
        };
        if (expectedType is null
            || !string.Equals(request.ContentType, expectedType, StringComparison.OrdinalIgnoreCase)
            || !await HasExpectedSignatureAsync(request.Content, extension, cancellationToken))
        {
            return ExperienceEvidenceMutationResult.InvalidFile;
        }

        var editor = await repository.GetDraftEditorAsync(request.UserId, request.ExperienceId, cancellationToken);
        if (editor is null) return ExperienceEvidenceMutationResult.ExperienceUnavailable;
        var count = editor.Files.Count(item => item.Kind == request.Kind);
        var maxCount = request.Kind switch
        {
            EvidenceKind.FoodAreaPhoto => 5,
            EvidenceKind.PortfolioPhoto => 10,
            EvidenceKind.WorkshopPhoto => 5,
            EvidenceKind.VenueProof or EvidenceKind.VenuePermission => 1,
            _ => 0
        };
        var correctType = editor.ExperienceType switch
        {
            EvidenceExperienceType.Food => request.Kind == EvidenceKind.FoodAreaPhoto,
            EvidenceExperienceType.Arts => request.Kind is EvidenceKind.PortfolioPhoto or EvidenceKind.WorkshopPhoto,
            EvidenceExperienceType.LocalTraditions => request.Kind is EvidenceKind.VenueProof or EvidenceKind.VenuePermission,
            _ => false
        };
        if (!correctType) return ExperienceEvidenceMutationResult.InvalidEvidenceKind;
        if (maxCount == 0 || count >= maxCount) return ExperienceEvidenceMutationResult.FileLimitReached;

        var storageKey = $"experiences/{request.ExperienceId}/evidence/{Guid.NewGuid():N}{extension}";
        request.Content.Position = 0;
        await storage.UploadAsync(storageKey, expectedType, request.Content, cancellationToken);
        try
        {
            var result = await repository.AddFileAsync(request.UserId, request.ExperienceId, request.Kind, storageKey, cancellationToken);
            if (result != ExperienceEvidenceMutationResult.Success) await TryDeleteAsync(storageKey, cancellationToken);
            return result;
        }
        catch
        {
            await TryDeleteAsync(storageKey, cancellationToken);
            throw;
        }
    }

    private static async Task<bool> HasExpectedSignatureAsync(Stream stream, string extension, CancellationToken cancellationToken)
    {
        var bytes = new byte[8];
        var read = await stream.ReadAsync(bytes.AsMemory(), cancellationToken);
        stream.Position = 0;
        return extension switch
        {
            ".pdf" => read >= 5 && bytes.AsSpan(0, 5).SequenceEqual("%PDF-"u8),
            ".jpg" or ".jpeg" => read >= 3 && bytes[0] == 0xff && bytes[1] == 0xd8 && bytes[2] == 0xff,
            ".png" => read >= 8 && bytes.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a }),
            _ => false
        };
    }

    private async Task TryDeleteAsync(string storageKey, CancellationToken cancellationToken)
    {
        try
        {
            await storage.DeleteAsync(storageKey, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Could not clean up private experience evidence object {StorageKey}.", storageKey);
        }
    }
}
