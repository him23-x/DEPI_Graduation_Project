using DiscoverEgypt.Application.Features.Disputes.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Disputes.Validators
{
    public sealed class OpenTouristDisputeCommandValidator : AbstractValidator<OpenTouristDisputeCommand>
    {
        public OpenTouristDisputeCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.Form).NotNull().DependentRules(() =>
            {
                RuleFor(command => command.Form.BookingId).GreaterThan(0);
                RuleFor(command => command.Form.Description)
                    .Must(description => !string.IsNullOrWhiteSpace(description))
                    .WithMessage("Describe the issue before submitting the dispute.")
                    .MaximumLength(5000);
            });
        }
    }
}
