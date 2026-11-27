using DiscoverEgypt.Application.Responses.AdminMonitoring;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IAdminMonitoringRepository
    {
        Task<AdminMonitoringDashboardResponse> GetSummaryAsync(CancellationToken cancellationToken = default);
    }
}
