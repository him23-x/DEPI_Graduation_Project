using DiscoverEgypt.Application.Responses.AdminExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminExperiences.Requests.Queries
{
    public sealed record GetAdminExperienceDetailsQuery(int ExperienceId)
        : IRequest<AdminExperienceDetailsResponse?>;
}
