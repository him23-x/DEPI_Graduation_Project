using DiscoverEgypt.Application.ViewModels.HostExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries
{
    public sealed record GetEditableHostExperienceDraftQuery(string UserId, int ExperienceId)
        : IRequest<HostExperienceDraftEditorViewModel?>;
}

