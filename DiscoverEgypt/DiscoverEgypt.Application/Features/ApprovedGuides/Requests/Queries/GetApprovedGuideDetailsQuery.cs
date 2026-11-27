using DiscoverEgypt.Application.DTOs.Guides;
using MediatR;

namespace DiscoverEgypt.Application.Features.ApprovedGuides.Requests.Queries
{
    public sealed record GetApprovedGuideDetailsQuery(int GuideProfileId) : IRequest<GuideProfileDetailsDto?>;
}
