using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Earnings;
using DiscoverEgypt.Application.Features.Earnings.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.Earnings.Handlers.Queries;

public sealed class GetHostEarningsQueryHandler(IHostEarningsRepository repository)
    : IRequestHandler<GetHostEarningsQuery, HostEarningsDto>
{
    public Task<HostEarningsDto> Handle(GetHostEarningsQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.GetForHostAsync(
            request.UserId,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);
    }
}
