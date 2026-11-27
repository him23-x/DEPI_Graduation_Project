using DiscoverEgypt.Application.Features.AdminDisputes.Requests.Commands;
using DiscoverEgypt.Domain.Enums.Disputes;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.AdminDisputes.Validators
{
    public sealed class ResolveAdminDisputeCommandValidator : AbstractValidator<ResolveAdminDisputeCommand>
    {
        public ResolveAdminDisputeCommandValidator()
        {
            RuleFor(command => command.AdminUserId).NotEmpty();
            RuleFor(command => command.DisputeId).GreaterThan(0);
            RuleFor(command => command.Form).NotNull();
            When(command => command.Form is not null, () =>
            {
                RuleFor(command => command.Form.Outcome)
                    .NotNull()
                    .Must(outcome => outcome.HasValue && Enum.IsDefined(outcome.Value))
                    .WithMessage("Choose a valid dispute outcome.");
                RuleFor(command => command.Form.ResolutionNote).MaximumLength(3000);
                RuleFor(command => command.Form.SanctionDays)
                    .GreaterThan(0)
                    .When(command => command.Form.SanctionDays.HasValue
                        && command.Form.Outcome != DisputeOutcome.NoFault)
                    .WithMessage("Sanction duration must be greater than zero days.");
            });
        }
    }
}
