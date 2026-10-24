using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

public sealed record UpdateHostExperienceDraftCommand(
    string UserId,
    int ExperienceId,
    CreateHostExperienceDraftRequest Draft) : IRequest<bool>;
