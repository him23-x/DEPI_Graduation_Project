using DiscoverEgypt.Application.ViewModels.HostExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries
{
    public sealed record GetHostExperienceMediaEditorQuery(string UserId, int ExperienceId)
        : IRequest<ExperienceMediaViewModel?>;
}

