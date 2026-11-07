using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.HostExperiences.Validators.Commands
{
    public sealed class SubmitHostExperienceCommandValidator : AbstractValidator<SubmitHostExperienceCommand>
    {
        public SubmitHostExperienceCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.ExperienceId).GreaterThan(0);
            RuleFor(command => command.Accepted).Equal(true).WithMessage("Accept the declaration before submitting.");
            RuleFor(command => command.VenueUse)
                .Must(value => !value.HasValue || Enum.IsDefined(value.Value))
                .WithMessage("Choose a valid venue type.");
        }
    }
}
