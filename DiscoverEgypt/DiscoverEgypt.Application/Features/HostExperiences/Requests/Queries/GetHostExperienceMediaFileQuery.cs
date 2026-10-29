using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

using DiscoverEgypt.Application.Contracts.Storage;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;

public sealed record GetHostExperienceMediaFileQuery(string UserId, int MediaId)
    : IRequest<ExperienceMediaDownload?>;
