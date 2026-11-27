using MediatR;

namespace DiscoverEgypt.Application.Features.ApprovedGuides.Requests.Queries
{
    public sealed record GetApprovedGuideImageKeyQuery(int GuideProfileId) : IRequest<string?>;
}
