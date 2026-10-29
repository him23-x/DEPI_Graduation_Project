using DiscoverEgypt.Application.DTOs.Dashboard;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostDashboardRepository
{
    Task<HostDashboardDto> GetForHostAsync(
        string userId,
        DateTime utcNow,
        CancellationToken cancellationToken = default);
}
