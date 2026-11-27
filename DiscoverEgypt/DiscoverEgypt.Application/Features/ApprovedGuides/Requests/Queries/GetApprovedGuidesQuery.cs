using DiscoverEgypt.Application.Responses.Guides;
using MediatR;

namespace DiscoverEgypt.Application.Features.ApprovedGuides.Requests.Queries
{
    public sealed record GetApprovedGuidesQuery(string? Search, int? LanguageId, int? CulturalAreaId, int Page = 1, int PageSize = 9) : IRequest<ApprovedGuidesResponse>;
}
