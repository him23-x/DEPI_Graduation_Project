using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands
{
    public sealed class AddHostExperienceEvidenceFileCommandHandler : IRequestHandler<AddHostExperienceEvidenceFileCommand, ExperienceEvidenceMutationOutcome>
    {
        private readonly IHostExperienceEvidenceRepository _repository;
        private readonly IExperienceEvidenceStorage _storage;
        private readonly ILogger<AddHostExperienceEvidenceFileCommandHandler> _logger;
        private const long MaxFileLength = 10 * 1024 * 1024;

        public AddHostExperienceEvidenceFileCommandHandler(
            IHostExperienceEvidenceRepository repository,
            IExperienceEvidenceStorage storage,
            ILogger<AddHostExperienceEvidenceFileCommandHandler> logger)
        {
            _repository = repository;
            _storage = storage;
            _logger = logger;
        }

        public async Task<ExperienceEvidenceMutationOutcome> Handle(AddHostExperienceEvidenceFileCommand request, CancellationToken cancellationToken)
        {
            if (request.Length <= 0 || request.Length > MaxFileLength || request.Content is null || !request.Content.CanSeek)
            {
                return ExperienceEvidenceMutationOutcome.InvalidFile;
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
                return ExperienceEvidenceMutationOutcome.InvalidFile;
            }

            var editor = await _repository.GetDraftEditorAsync(request.UserId, request.ExperienceId, cancellationToken);
            if (editor is null) return ExperienceEvidenceMutationOutcome.ExperienceUnavailable;
            var count = editor.Evidence.Count(item => item.EvidenceKind == request.Kind && item.DeletedAt == null && item.StorageKey is not null);
            var maxCount = HostExperienceSubmissionPolicy.GetFileLimit(request.Kind);
            var correctType = HostExperienceSubmissionPolicy.IsAllowedFileKind(
                HostExperienceSubmissionPolicy.Classify(editor.ExperienceType.Name), request.Kind);
            if (!correctType) return ExperienceEvidenceMutationOutcome.InvalidEvidenceKind;
            if (maxCount == 0 || count >= maxCount) return ExperienceEvidenceMutationOutcome.FileLimitReached;

            var storageKey = $"experiences/{request.ExperienceId}/evidence/{Guid.NewGuid():N}{extension}";
            request.Content.Position = 0;
            await _storage.UploadAsync(storageKey, expectedType, request.Content, cancellationToken);
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
                            return Task.FromResult(ExperienceEvidenceMutationOutcome.ExperienceUnavailable);
                        }

                        var type = HostExperienceSubmissionPolicy.Classify(experience.ExperienceType.Name);
                        if (!HostExperienceSubmissionPolicy.IsAllowedFileKind(type, request.Kind))
                        {
                            return Task.FromResult(ExperienceEvidenceMutationOutcome.InvalidEvidenceKind);
                        }

                        var limit = HostExperienceSubmissionPolicy.GetFileLimit(request.Kind);
                        var count = experience.Evidence.Count(item =>
                            item.EvidenceKind == request.Kind
                            && item.DeletedAt == null
                            && item.StorageKey != null);
                        if (limit == 0 || count >= limit)
                        {
                            return Task.FromResult(ExperienceEvidenceMutationOutcome.FileLimitReached);
                        }

                        experience.Evidence.Add(new ExperienceEvidence
                        {
                            EvidenceKind = request.Kind,
                            StorageKey = storageKey,
                            UploadedAt = DateTime.UtcNow
                        });
                        return Task.FromResult(ExperienceEvidenceMutationOutcome.Success);
                    },
                    cancellationToken);
                if (result != ExperienceEvidenceMutationOutcome.Success) await TryDeleteAsync(storageKey, cancellationToken);
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
                await _storage.DeleteAsync(storageKey, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogError(exception, "Could not clean up private experience evidence object {StorageKey}.", storageKey);
            }
        }
    }
}

