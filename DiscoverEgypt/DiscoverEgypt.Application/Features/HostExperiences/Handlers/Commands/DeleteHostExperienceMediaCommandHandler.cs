using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class DeleteHostExperienceMediaCommandHandler(
    IHostExperienceMediaRepository repository,
    IExperienceMediaStorage storage,
    ILogger<DeleteHostExperienceMediaCommandHandler> logger)
    : IRequestHandler<DeleteHostExperienceMediaCommand, ExperienceMediaMutationResult>
{
    public async Task<ExperienceMediaMutationResult> Handle(DeleteHostExperienceMediaCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        var (result, storageKey) = await repository.DeleteAsync(request.UserId, request.ExperienceId, request.MediaId, cancellationToken);
        if (result == ExperienceMediaMutationResult.Success && storageKey is not null)
        {
            try
            {
                await storage.DeleteAsync(storageKey, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "Could not delete private experience media object {StorageKey}.", storageKey);
            }
        }

        return result;
    }
}
