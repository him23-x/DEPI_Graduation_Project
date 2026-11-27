using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.HostExperiences;
using DiscoverEgypt.Application.Enums.AdminExperiences;
using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.AdminExperiences.Requests.Commands;
using DiscoverEgypt.Application.Responses.AdminExperiences;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums.Experiences;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminExperiences.Handlers.Commands
{
    public sealed class DecideAdminExperienceReviewCommandHandler
        : IRequestHandler<DecideAdminExperienceReviewCommand, ExperienceReviewDecisionResponse>
    {
        private readonly IAdminExperienceReviewRepository _repository;
        private readonly IValidator<DecideAdminExperienceReviewCommand> _validator;
        private readonly TimeProvider _timeProvider;
        private readonly IExperienceReadinessService _readinessService;

        public DecideAdminExperienceReviewCommandHandler(
            IAdminExperienceReviewRepository repository,
            IValidator<DecideAdminExperienceReviewCommand> validator,
            TimeProvider timeProvider,
            IExperienceReadinessService readinessService)
        {
            _repository = repository;
            _validator = validator;
            _timeProvider = timeProvider;
            _readinessService = readinessService;
        }

        public async Task<ExperienceReviewDecisionResponse> Handle(
            DecideAdminExperienceReviewCommand request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            if (request.Decision == Application.Enums.AdminExperiences.ExperienceReviewDecision.Republish)
            {
                var experience = await _repository.GetDetailsAsync(request.ExperienceId, cancellationToken);
                if (experience is null)
                    return new ExperienceReviewDecisionResponse(ExperienceReviewDecisionOutcome.ExperienceUnavailable, request.ExperienceId, []);
                if (experience.Status != ExperienceStatus.Unpublished)
                    return new ExperienceReviewDecisionResponse(ExperienceReviewDecisionOutcome.NotUnpublished, request.ExperienceId, []);

                var issues = GetRepublishIssues(experience);
                if (issues.Count > 0)
                    return new ExperienceReviewDecisionResponse(
                        ExperienceReviewDecisionOutcome.RepublishPrerequisitesNotMet,
                        request.ExperienceId,
                        issues);
            }

            var reason = request.Decision == Application.Enums.AdminExperiences.ExperienceReviewDecision.Reject
                ? request.Form.RejectionReason!.Trim()
                : null;
            var outcome = await _repository.DecideAsync(
                request.AdminUserId,
                request.ExperienceId,
                request.Decision,
                reason,
                _timeProvider.GetUtcNow().UtcDateTime,
                cancellationToken);

            return new ExperienceReviewDecisionResponse(outcome, request.ExperienceId, []);
        }

        private IReadOnlyList<string> GetRepublishIssues(AdminExperienceDetailsResponse experience)
        {
            var evidence = experience.Evidence
                .GroupBy(item => item.Kind)
                .Select(group => new ExperienceEvidenceSummaryDto(
                    group.Key,
                    group.Select(item => item.TextContent).FirstOrDefault(text => !string.IsNullOrWhiteSpace(text)),
                    group.Count(item => item.HasFile)))
                .ToArray();
            var experienceType = HostExperienceSubmissionPolicy.Classify(experience.ExperienceType);
            var review = new ExperienceSubmissionReviewDto(
                experience.ExperienceId,
                experience.Title,
                experience.ExperienceType,
                experienceType,
                experience.CulturalArea,
                experience.Description,
                experience.CulturalStory,
                experience.PricePerPerson,
                experience.DurationMinutes,
                experience.MaxGroupSize,
                experience.MeetingPointArea,
                experience.ExactMeetingPoint,
                experience.Status,
                experience.RejectionReason,
                experience.ImageCount,
                experience.VideoCount,
                experience.HasCoverImage,
                evidence,
                experience.Interests.Count > 0,
                experience.Languages.Count > 0,
                []);

            var issues = _readinessService.GetIssues(review).ToList();
            if (experience.ImageCount > 8) issues.Add("An experience can have no more than 8 photos.");
            if (experience.VideoCount > 2) issues.Add("An experience can have no more than 2 videos.");
            if (experience.CoverImageCount > 1) issues.Add("Choose only one cover photo.");

            var declaration = HostExperienceSubmissionPolicy.GetDeclaration(experienceType);
            if (declaration is not null && !experience.Declarations.Any(item =>
                    item.Type == declaration.Type && item.Text == declaration.Text))
                issues.Add("Accept the current required safety and venue declaration before republishing.");

            return issues;
        }
    }
}
