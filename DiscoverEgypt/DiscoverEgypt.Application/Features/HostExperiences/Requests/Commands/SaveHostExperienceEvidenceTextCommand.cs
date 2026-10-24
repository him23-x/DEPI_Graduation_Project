using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

public sealed record SaveHostExperienceEvidenceTextCommand(string UserId, int ExperienceId, EvidenceKind Kind, string? Text)
    : IRequest<ExperienceEvidenceMutationResult>;
