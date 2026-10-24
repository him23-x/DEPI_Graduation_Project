using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

public sealed record DeleteHostExperienceEvidenceFileCommand(string UserId, int ExperienceId, int EvidenceId)
    : IRequest<ExperienceEvidenceMutationResult>;
