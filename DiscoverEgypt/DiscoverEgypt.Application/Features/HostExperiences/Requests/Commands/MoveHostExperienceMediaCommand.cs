using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

public sealed record MoveHostExperienceMediaCommand(string UserId, int ExperienceId, int MediaId, int Direction)
    : IRequest<ExperienceMediaMutationOutcome>;
