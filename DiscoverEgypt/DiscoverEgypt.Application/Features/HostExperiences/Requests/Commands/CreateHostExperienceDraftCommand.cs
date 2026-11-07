using DiscoverEgypt.Application.ViewModels.HostExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands
{
    public sealed record CreateHostExperienceDraftCommand(
        string UserId,
        HostExperienceDraftViewModel Draft) : IRequest<int?>;
}

