using DiscoverEgypt.Application.Responses.Experiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.Explore.Requests.Queries
{
    public sealed record ExploreExperiencesQuery(
        string? Search = null,
        int? TypeId = null,
        int? AreaId = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        string? Sort = null,
        int Page = 1,
        int PageSize = 9) : IRequest<ExploreExperiencesResponse>;
}
