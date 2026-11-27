using DiscoverEgypt.Application.Responses.AdminMonitoring;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminMonitoring.Requests.Queries
{
    public sealed record GetAdminMonitoringDashboardQuery : IRequest<AdminMonitoringDashboardResponse>;
}
