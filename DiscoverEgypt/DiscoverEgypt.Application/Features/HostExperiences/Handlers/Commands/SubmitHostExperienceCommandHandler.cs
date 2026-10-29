using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Validators.DTOs;
using MediatR;

using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class SubmitHostExperienceCommandHandler(
    IHostExperienceSubmissionRepository repository,
    HostExperienceSubmissionReviewValidator reviewValidator)
    : IRequestHandler<SubmitHostExperienceCommand, ExperienceSubmissionOutcome>
{
    public async Task<ExperienceSubmissionOutcome> Handle(SubmitHostExperienceCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        if (!request.Accepted) return ExperienceSubmissionOutcome.DeclarationNotAccepted;

        var review = await repository.GetReviewAsync(request.UserId, request.ExperienceId, cancellationToken);
        if (review is null) return ExperienceSubmissionOutcome.ExperienceUnavailable;
        var declaration = HostExperienceSubmissionPolicy.GetDeclaration(review.ExperienceType);
        if (declaration is null) return ExperienceSubmissionOutcome.UnsupportedType;
        if (review.ExperienceType == EvidenceExperienceType.LocalTraditions
            && (!request.VenueUse.HasValue || !Enum.IsDefined(request.VenueUse.Value)))
        {
            return ExperienceSubmissionOutcome.Incomplete;
        }
        if (reviewValidator.GetIssues(review, request.VenueUse, request.EvidenceInHostName).Count > 0)
        {
            return ExperienceSubmissionOutcome.Incomplete;
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
