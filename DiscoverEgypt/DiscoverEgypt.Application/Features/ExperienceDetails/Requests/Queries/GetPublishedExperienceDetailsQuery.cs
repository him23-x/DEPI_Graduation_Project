using DiscoverEgypt.Application.DTOs.ExperienceDetails;
using MediatR;

namespace DiscoverEgypt.Application.Features.ExperienceDetails.Requests.Queries
{
    public sealed record GetPublishedExperienceDetailsQuery(int ExperienceId) : IRequest<ExperienceDetailsDto?>;
}
