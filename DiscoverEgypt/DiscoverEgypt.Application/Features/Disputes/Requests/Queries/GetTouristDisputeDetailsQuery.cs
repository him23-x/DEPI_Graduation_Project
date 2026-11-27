using DiscoverEgypt.Application.Responses.Disputes;
using MediatR;

namespace DiscoverEgypt.Application.Features.Disputes.Requests.Queries
{
    public sealed record GetTouristDisputeDetailsQuery(string UserId, int DisputeId)
        : IRequest<TouristDisputeDetailsResponse?>;
}
