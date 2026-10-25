using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Validation;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class SubmitHostExperienceCommandHandler(
    IHostExperienceSubmissionRepository repository,
    HostExperienceSubmissionReviewValidator reviewValidator)
    : IRequestHandler<SubmitHostExperienceCommand, ExperienceSubmissionResult>
{
    public async Task<ExperienceSubmissionResult> Handle(SubmitHostExperienceCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        if (!request.Accepted) return ExperienceSubmissionResult.DeclarationNotAccepted;

        var review = await repository.GetReviewAsync(request.UserId, request.ExperienceId, cancellationToken);
        if (review is null) return ExperienceSubmissionResult.ExperienceUnavailable;
        var declaration = HostExperienceSubmissionPolicy.GetDeclaration(review.ExperienceType);
        if (declaration is null) return ExperienceSubmissionResult.UnsupportedType;
        if (review.ExperienceType == EvidenceExperienceType.LocalTraditions
            && (!request.VenueUse.HasValue || !Enum.IsDefined(request.VenueUse.Value)))
        {
            return ExperienceSubmissionResult.Incomplete;
        }
        if (reviewValidator.GetIssues(review, request.VenueUse, request.EvidenceInHostName).Count > 0)
        {
            return ExperienceSubmissionResult.Incomplete;
        }

        return await repository.SubmitAsync(
            request.UserId,
            request.ExperienceId,
            declaration.Type,
            declaration.Text,
            request.VenueUse,
            request.EvidenceInHostName,
            cancellationToken);
    }
}
