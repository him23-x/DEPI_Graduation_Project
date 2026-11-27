using MediatR;

namespace DiscoverEgypt.Application.Features.ExperienceDetails.Requests.Queries
{
    public sealed record GetPublishedExperienceDetailsPhotoKeyQuery(int ExperienceId, int MediaId) : IRequest<string?>;
}
