using DiscoverEgypt.Application.Enums.GuideVerification;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.AdminGuideVerification.Validators
{
    public sealed class DecideAdminGuideVerificationCommandValidator
        : AbstractValidator<DecideAdminGuideVerificationCommand>
    {
        public DecideAdminGuideVerificationCommandValidator()
        {
            RuleFor(command => command.AdminUserId).NotEmpty();
            RuleFor(command => command.VerificationRequestId).GreaterThan(0);
            RuleFor(command => command.Decision).IsInEnum();
            RuleFor(command => command.Form).NotNull();
            When(command => command.Form is not null
                && command.Decision == GuideVerificationDecision.Reject, () =>
            {
                RuleFor(command => command.Form.RejectionReason)
                    .Must(reason => !string.IsNullOrWhiteSpace(reason))
                    .WithMessage("Enter a reason for rejecting this guide verification request.")
                    .MaximumLength(1000);
            });
        }
    }
}
