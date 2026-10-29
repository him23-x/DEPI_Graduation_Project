using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Validators.DTOs;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries;

public sealed class GetHostExperienceSubmissionReviewQueryHandler(
    IHostExperienceSubmissionRepository repository,
    HostExperienceSubmissionReviewValidator reviewValidator)
    : IRequestHandler<GetHostExperienceSubmissionReviewQuery, HostExperienceSubmissionReviewDto?>
{
    public async Task<HostExperienceSubmissionReviewDto?> Handle(GetHostExperienceSubmissionReviewQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        var review = await repository.GetReviewAsync(request.UserId, request.ExperienceId, cancellationToken);
        return review is null ? null : review with { Issues = reviewValidator.GetIssues(review) };
    }
}
