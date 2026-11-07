using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands
{
    public sealed record ReturnPublishedExperienceToReviewCommand(string UserId, int ExperienceId) : IRequest<bool>;
}
