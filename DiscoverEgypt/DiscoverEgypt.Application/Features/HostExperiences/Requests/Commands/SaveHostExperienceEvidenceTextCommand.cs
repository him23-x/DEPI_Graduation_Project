using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

public sealed record SaveHostExperienceEvidenceTextCommand(string UserId, int ExperienceId, EvidenceKind Kind, string? Text)
    : IRequest<ExperienceEvidenceMutationOutcome>;
