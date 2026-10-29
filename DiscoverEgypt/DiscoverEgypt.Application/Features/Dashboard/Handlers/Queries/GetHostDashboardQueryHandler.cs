using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Dashboard;
using DiscoverEgypt.Application.Features.Dashboard.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.Dashboard.Handlers.Queries;

public sealed class GetHostDashboardQueryHandler(
    IHostDashboardRepository repository,
    TimeProvider timeProvider) : IRequestHandler<GetHostDashboardQuery, HostDashboardDto>
{
    public Task<HostDashboardDto> Handle(GetHostDashboardQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.GetForHostAsync(
            request.UserId,
            timeProvider.GetUtcNow().UtcDateTime,
            cancellationToken);
    }
}
