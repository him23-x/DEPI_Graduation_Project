using DiscoverEgypt.Application.Responses.Dashboard;
using MediatR;

namespace DiscoverEgypt.Application.Features.Dashboard.Requests.Queries
{
    public sealed record GetTouristDashboardQuery(string UserId) : IRequest<TouristDashboardResponse>;
}
