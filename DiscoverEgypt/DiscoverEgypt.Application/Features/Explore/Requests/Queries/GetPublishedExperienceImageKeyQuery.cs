using MediatR;

namespace DiscoverEgypt.Application.Features.Explore.Requests.Queries
{
    public sealed record GetPublishedExperienceImageKeyQuery(int ExperienceId) : IRequest<string?>;
}
