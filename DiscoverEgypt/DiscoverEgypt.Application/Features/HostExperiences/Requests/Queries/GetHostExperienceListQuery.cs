using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;

public sealed record GetHostExperienceListQuery(
    string UserId,
    ExperienceStatus? Status) : IRequest<IReadOnlyList<HostExperienceListItemDto>>;
