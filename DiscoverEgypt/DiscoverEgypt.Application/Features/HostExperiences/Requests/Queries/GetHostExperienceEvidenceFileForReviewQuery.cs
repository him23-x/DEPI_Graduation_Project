using DiscoverEgypt.Application.Contracts.Storage;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;

public sealed record GetHostExperienceEvidenceFileForReviewQuery(int EvidenceId)
    : IRequest<ExperienceEvidenceDownload?>;
