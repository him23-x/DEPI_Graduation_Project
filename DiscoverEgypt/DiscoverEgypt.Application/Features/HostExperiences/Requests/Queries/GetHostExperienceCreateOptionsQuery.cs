using DiscoverEgypt.Application.ViewModels.HostExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries
{
    public sealed record GetHostExperienceCreateOptionsQuery : IRequest<ExperienceCreateOptionsViewModel>;
}

