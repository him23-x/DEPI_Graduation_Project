using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Disputes;
using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.Disputes.Handlers.Queries;

public sealed class GetHostDisputeDetailsQueryHandler(IHostDisputesRepository repository)
    : IRequestHandler<GetHostDisputeDetailsQuery, HostDisputeDetailsDto?>
{
    public Task<HostDisputeDetailsDto?> Handle(GetHostDisputeDetailsQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        if (request.DisputeId <= 0) return Task.FromResult<HostDisputeDetailsDto?>(null);
        return repository.GetDetailsAsync(request.UserId, request.DisputeId, cancellationToken);
    }
}
