using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

public sealed record AddHostExperienceMediaCommand(
    string UserId,
    int ExperienceId,
    string FileName,
    string ContentType,
    long Length,
    Stream Content) : IRequest<ExperienceMediaMutationOutcome>;
