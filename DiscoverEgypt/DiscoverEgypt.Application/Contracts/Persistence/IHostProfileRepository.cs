using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Hosts;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IHostProfileRepository
    {
        Task<HostProfile?> GetForUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<string?> SaveAsync(string userId, HostType hostType, string bio, string? profileImageKey = null, CancellationToken cancellationToken = default);
    }
}
