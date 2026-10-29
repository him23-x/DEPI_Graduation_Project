using DiscoverEgypt.Application.Contracts.HostExperiences;
using DiscoverEgypt.Application.Contracts.Persistence;

namespace DiscoverEgypt.Application.Features.HostExperiences.Services;

public sealed class ExperienceStatusService(IExperienceStatusRepository repository)
    : IExperienceStatusService
{
    public Task<bool> ReturnToPendingApprovalAsync(
        string userId,
        int experienceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        if (experienceId <= 0)
            throw new ArgumentOutOfRangeException(nameof(experienceId));

        return repository.ReturnToPendingApprovalAsync(userId, experienceId, cancellationToken);
    }
}
