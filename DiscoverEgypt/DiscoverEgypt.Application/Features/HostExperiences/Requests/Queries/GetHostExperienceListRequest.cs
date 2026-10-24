using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;

public sealed record GetHostExperienceListRequest(
    string UserId,
    ExperienceStatus? Status) : IRequest<IReadOnlyList<HostExperienceListItem>>;
