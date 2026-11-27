using DiscoverEgypt.Application.Responses.Recommendations;
using MediatR;

namespace DiscoverEgypt.Application.Features.Recommendations.Requests.Queries
{
    public sealed record GetRecommendedExperiencesQuery(string UserId, int? TypeId = null)
        : IRequest<RecommendedExperiencesResponse>;
}
