using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using DiscoverEgypt.Application.Features.HostExperiences.Validators;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries;

public sealed class GetHostExperienceSubmissionReviewQueryHandler(IHostExperienceSubmissionRepository repository)
    : IRequestHandler<GetHostExperienceSubmissionReviewQuery, HostExperienceSubmissionReview?>
{
    public async Task<HostExperienceSubmissionReview?> Handle(GetHostExperienceSubmissionReviewQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        var review = await repository.GetReviewAsync(request.UserId, request.ExperienceId, cancellationToken);
        return review is null ? null : review with { Issues = HostExperienceSubmissionRules.GetIssues(review) };
    }
}
