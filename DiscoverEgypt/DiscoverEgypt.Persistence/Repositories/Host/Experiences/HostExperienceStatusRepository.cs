using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Domain.Enums.Experiences;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Host.Experiences;

public sealed class HostExperienceStatusRepository(DiscoverEgyptDbContext dbContext)
    : IExperienceStatusRepository
{
    public async Task<bool> ReturnToPendingApprovalAsync(
        string userId,
        int experienceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var experience = await dbContext.Experiences.SingleOrDefaultAsync(
            item => item.Id == experienceId
                && item.HostProfile.UserId == userId
                && item.Status == ExperienceStatus.Published,
            cancellationToken);

        if (experience is null)
            return false;

        experience.Status = ExperienceStatus.PendingApproval;
        experience.RejectionReason = null;
        experience.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
