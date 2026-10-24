using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

public sealed record DeleteHostExperienceMediaCommand(string UserId, int ExperienceId, int MediaId)
    : IRequest<ExperienceMediaMutationResult>;
