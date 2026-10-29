using DiscoverEgypt.Application.DTOs.HostExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;

public sealed record GetHostExperienceSubmissionReviewQuery(string UserId, int ExperienceId)
    : IRequest<HostExperienceSubmissionReviewDto?>;
