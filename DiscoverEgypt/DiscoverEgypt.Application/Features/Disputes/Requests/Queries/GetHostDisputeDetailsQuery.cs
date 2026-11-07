using DiscoverEgypt.Application.ViewModels.Disputes;
using MediatR;

namespace DiscoverEgypt.Application.Features.Disputes.Requests.Queries
{
    public sealed record GetHostDisputeDetailsQuery(string UserId, int DisputeId)
        : IRequest<HostDisputeDetailsViewModel?>;
}

