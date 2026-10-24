using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;

public sealed record GetHostExperienceEvidenceFileQuery(string UserId, int EvidenceId)
    : IRequest<ExperienceEvidenceDownload?>;
