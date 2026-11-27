using DiscoverEgypt.Application.DTOs.AdminExperiences;
using DiscoverEgypt.Application.Enums.AdminExperiences;
using DiscoverEgypt.Application.Responses.AdminExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminExperiences.Requests.Commands
{
    public sealed record DecideAdminExperienceReviewCommand(
        string AdminUserId,
        int ExperienceId,
        ExperienceReviewDecision Decision,
        AdminExperienceDecisionDto Form)
        : IRequest<ExperienceReviewDecisionResponse>;
}
