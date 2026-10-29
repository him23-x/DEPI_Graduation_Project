using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

public sealed record CreateHostExperienceDraftCommand(
    string UserId,
    CreateHostExperienceDraftDto Draft) : IRequest<int?>;
