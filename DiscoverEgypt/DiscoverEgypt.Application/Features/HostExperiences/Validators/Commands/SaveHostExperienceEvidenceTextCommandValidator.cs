using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Domain.Enums.Experiences;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.HostExperiences.Validators.Commands
{
    public sealed class SaveHostExperienceEvidenceTextCommandValidator : AbstractValidator<SaveHostExperienceEvidenceTextCommand>
    {
        public SaveHostExperienceEvidenceTextCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.ExperienceId).GreaterThan(0);
            RuleFor(command => command.Kind).IsInEnum();
            RuleFor(command => command.Text).MaximumLength(5000);
            RuleFor(command => command.Text)
                .NotEmpty()
                .When(command => command.Kind is EvidenceKind.MenuAndAllergenInfo or EvidenceKind.RouteDetails)
                .WithMessage("This evidence text is required.");
        }
    }
}
