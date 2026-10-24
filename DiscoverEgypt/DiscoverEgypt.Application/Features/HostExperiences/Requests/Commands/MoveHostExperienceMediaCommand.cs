using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

public sealed record MoveHostExperienceMediaCommand(string UserId, int ExperienceId, int MediaId, int Direction)
    : IRequest<ExperienceMediaMutationResult>;
