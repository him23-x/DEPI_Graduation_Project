using DiscoverEgypt.Application.ViewModels.HostExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands
{
    public sealed record UpdateHostExperienceDraftCommand(
        string UserId,
        int ExperienceId,
        HostExperienceDraftViewModel Draft) : IRequest<bool>;
}

