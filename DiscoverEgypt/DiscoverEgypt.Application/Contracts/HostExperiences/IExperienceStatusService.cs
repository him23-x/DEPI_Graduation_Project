namespace DiscoverEgypt.Application.Contracts.HostExperiences;

public interface IExperienceStatusService
{
    Task<bool> ReturnToPendingApprovalAsync(
        string userId,
        int experienceId,
        CancellationToken cancellationToken = default);
}
