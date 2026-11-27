using DiscoverEgypt.Application.Enums.AdminExperiences;
using DiscoverEgypt.Application.Features.AdminExperiences.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.AdminExperiences.Validators
{
    public sealed class DecideAdminExperienceReviewCommandValidator
        : AbstractValidator<DecideAdminExperienceReviewCommand>
    {
        public DecideAdminExperienceReviewCommandValidator()
        {
            RuleFor(command => command.AdminUserId).NotEmpty();
            RuleFor(command => command.ExperienceId).GreaterThan(0);
            RuleFor(command => command.Decision).IsInEnum();
            RuleFor(command => command.Form).NotNull();
            When(command => command.Form is not null
                && command.Decision == ExperienceReviewDecision.Reject, () =>
            {
                RuleFor(command => command.Form.RejectionReason)
                    .Must(reason => !string.IsNullOrWhiteSpace(reason))
                    .WithMessage("Enter a reason for rejecting this experience.")
                    .MaximumLength(1000);
            });
        }
    }
}
