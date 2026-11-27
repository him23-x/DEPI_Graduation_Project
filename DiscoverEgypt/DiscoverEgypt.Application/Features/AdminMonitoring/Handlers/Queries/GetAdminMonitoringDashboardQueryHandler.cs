using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.AdminMonitoring.Requests.Queries;
using DiscoverEgypt.Application.Responses.AdminMonitoring;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminMonitoring.Handlers.Queries
{
    public sealed class GetAdminMonitoringDashboardQueryHandler
        : IRequestHandler<GetAdminMonitoringDashboardQuery, AdminMonitoringDashboardResponse>
    {
        private readonly IAdminMonitoringRepository _repository;
        private readonly TimeProvider _timeProvider;

        public GetAdminMonitoringDashboardQueryHandler(
            IAdminMonitoringRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<AdminMonitoringDashboardResponse> Handle(
            GetAdminMonitoringDashboardQuery request,
            CancellationToken cancellationToken)
        {
            var summary = await _repository.GetSummaryAsync(cancellationToken);
            return summary with { GeneratedAtUtc = _timeProvider.GetUtcNow().UtcDateTime };
        }
    }
}
