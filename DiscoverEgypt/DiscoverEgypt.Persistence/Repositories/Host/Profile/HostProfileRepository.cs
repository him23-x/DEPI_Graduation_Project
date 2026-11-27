using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Hosts;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Host.Profile
{
    public sealed class HostProfileRepository : IHostProfileRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public HostProfileRepository(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<HostProfile?> GetForUserAsync(string userId, CancellationToken cancellationToken = default) =>
            _dbContext.HostProfiles
                .AsNoTracking()
                .SingleOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);

        public async Task<string?> SaveAsync(string userId, HostType hostType, string bio, string? profileImageKey = null, CancellationToken cancellationToken = default)
        {
            var profile = await _dbContext.HostProfiles
                .SingleOrDefaultAsync(item => item.UserId == userId, cancellationToken);
            string? previousImageKey = null;

            if (profile is null)
            {
                profile = new HostProfile
                {
                    UserId = userId,
                    HostType = hostType,
                    Bio = bio.Trim(),
                    ProfileImageKey = profileImageKey,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.HostProfiles.Add(profile);
            }
            else
            {
                profile.HostType = hostType;
                profile.Bio = bio.Trim();
                if (!string.IsNullOrWhiteSpace(profileImageKey))
                {
                    previousImageKey = profile.ProfileImageKey;
                    profile.ProfileImageKey = profileImageKey;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return previousImageKey;
        }
    }
}
