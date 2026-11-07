using DiscoverEgypt.Application.ViewModels.Dashboard;
using MediatR;

namespace DiscoverEgypt.Application.Features.Dashboard.Requests.Queries
{
    public sealed record GetHostDashboardQuery(string UserId) : IRequest<HostDashboardViewModel>;
}

