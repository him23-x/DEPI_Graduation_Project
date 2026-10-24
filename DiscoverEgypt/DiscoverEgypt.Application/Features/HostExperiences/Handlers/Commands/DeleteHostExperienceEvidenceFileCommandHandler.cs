using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class DeleteHostExperienceEvidenceFileCommandHandler(
    IHostExperienceEvidenceRepository repository,
    IExperienceEvidenceStorage storage,
    ILogger<DeleteHostExperienceEvidenceFileCommandHandler> logger)
    : IRequestHandler<DeleteHostExperienceEvidenceFileCommand, ExperienceEvidenceMutationResult>
{
    public async Task<ExperienceEvidenceMutationResult> Handle(DeleteHostExperienceEvidenceFileCommand request, CancellationToken cancellationToken)
    {
        var (result, storageKey) = await repository.DeleteFileAsync(request.UserId, request.ExperienceId, request.EvidenceId, cancellationToken);
        if (result == ExperienceEvidenceMutationResult.Success && storageKey is not null)
        {
            try
            {
                await storage.DeleteAsync(storageKey, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "Could not delete private experience evidence object {StorageKey}.", storageKey);
            }
        }

        return result;
    }
}
