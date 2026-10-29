using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Disputes;
using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.Disputes.Handlers.Queries;

public sealed class GetHostDisputesQueryHandler(IHostDisputesRepository repository)
    : IRequestHandler<GetHostDisputesQuery, HostDisputesPageDto>
{
    public Task<HostDisputesPageDto> Handle(GetHostDisputesQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.GetForHostAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
    }
}
