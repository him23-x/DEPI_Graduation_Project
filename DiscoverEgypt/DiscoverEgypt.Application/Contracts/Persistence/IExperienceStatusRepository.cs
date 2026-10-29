namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IExperienceStatusRepository
{
    Task<bool> ReturnToPendingApprovalAsync(
        string userId,
        int experienceId,
        CancellationToken cancellationToken = default);
}
