using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

public sealed record AddHostExperienceEvidenceFileCommand(
    string UserId,
    int ExperienceId,
    EvidenceKind Kind,
    string FileName,
    string ContentType,
    long Length,
    Stream Content) : IRequest<ExperienceEvidenceMutationOutcome>;
