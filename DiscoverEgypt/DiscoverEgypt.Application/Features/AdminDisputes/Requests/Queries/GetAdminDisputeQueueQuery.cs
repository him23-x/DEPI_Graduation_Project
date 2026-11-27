using DiscoverEgypt.Application.Responses.Disputes;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminDisputes.Requests.Queries
{
    public sealed record GetAdminDisputeQueueQuery
        : IRequest<IReadOnlyList<AdminDisputeQueueItemResponse>>;
}
