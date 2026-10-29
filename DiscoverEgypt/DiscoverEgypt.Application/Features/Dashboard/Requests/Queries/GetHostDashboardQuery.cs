using DiscoverEgypt.Application.DTOs.Dashboard;
using MediatR;

namespace DiscoverEgypt.Application.Features.Dashboard.Requests.Queries;

public sealed record GetHostDashboardQuery(string UserId) : IRequest<HostDashboardDto>;
