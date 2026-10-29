using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

public sealed record SubmitHostExperienceCommand(
    string UserId,
    int ExperienceId,
    bool Accepted,
    ExperienceVenueUse? VenueUse,
    bool EvidenceInHostName) : IRequest<ExperienceSubmissionOutcome>;
