using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

public sealed record AddHostExperienceEvidenceFileCommand(
    string UserId,
    int ExperienceId,
    EvidenceKind Kind,
    string FileName,
    string ContentType,
    long Length,
    Stream Content) : IRequest<ExperienceEvidenceMutationResult>;
