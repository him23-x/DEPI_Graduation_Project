using DiscoverEgypt.Application.Responses.Disputes;
using MediatR;

namespace DiscoverEgypt.Application.Features.Disputes.Requests.Queries
{
    public sealed record GetTouristDisputesQuery(string UserId)
        : IRequest<IReadOnlyList<TouristDisputeListItemResponse>>;
}
