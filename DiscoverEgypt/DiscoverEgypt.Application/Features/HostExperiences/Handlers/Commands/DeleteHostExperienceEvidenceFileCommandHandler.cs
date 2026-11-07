using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands
{
    public sealed class DeleteHostExperienceEvidenceFileCommandHandler : IRequestHandler<DeleteHostExperienceEvidenceFileCommand, ExperienceEvidenceMutationOutcome>
    {
        private readonly IHostExperienceEvidenceRepository _repository;
        private readonly IExperienceEvidenceStorage _storage;
        private readonly ILogger<DeleteHostExperienceEvidenceFileCommandHandler> _logger;

        public DeleteHostExperienceEvidenceFileCommandHandler(
            IHostExperienceEvidenceRepository repository,
            IExperienceEvidenceStorage storage,
            ILogger<DeleteHostExperienceEvidenceFileCommandHandler> logger)
        {
            _repository = repository;
            _storage = storage;
            _logger = logger;
        }

        public async Task<ExperienceEvidenceMutationOutcome> Handle(
            DeleteHostExperienceEvidenceFileCommand request,
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
                        return Task.FromResult(ExperienceEvidenceMutationOutcome.ExperienceUnavailable);
                    }

                    var evidence = experience.Evidence.SingleOrDefault(item =>
                        item.Id == request.EvidenceId
                        && item.DeletedAt == null
                        && item.StorageKey != null);
                    if (evidence is null)
                    {
                        return Task.FromResult(ExperienceEvidenceMutationOutcome.EvidenceUnavailable);
                    }

                    storageKey = evidence.StorageKey;
                    evidence.DeletedAt = DateTime.UtcNow;
                    return Task.FromResult(ExperienceEvidenceMutationOutcome.Success);
                },
                cancellationToken);

            if (result == ExperienceEvidenceMutationOutcome.Success && storageKey is not null)
            {
                try
                {
                    await _storage.DeleteAsync(storageKey, cancellationToken);
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    _logger.LogError(exception, "Could not delete private experience evidence object {StorageKey}.", storageKey);
                }
            }

            return result;
        }
    }
}
