using DiscoverEgypt.Application.Responses.Reviews;
using MediatR;

namespace DiscoverEgypt.Application.Features.Reviews.Requests.Queries
{
    public sealed record GetHostReviewsQuery(string UserId, int Page = 1, int PageSize = 10)
        : IRequest<HostReviewsResponse>;
}

