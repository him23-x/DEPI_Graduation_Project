using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Validators
{
    public sealed class CreateExperienceBookingCommandValidator : AbstractValidator<CreateExperienceBookingCommand>
    {
        public CreateExperienceBookingCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.Form).NotNull();
            When(command => command.Form is not null, () =>
            {
                RuleFor(command => command.Form.ExperienceId).GreaterThan(0);
                RuleFor(command => command.Form.NumberOfGuests).NotNull().GreaterThan(0);
                RuleFor(command => command.Form.ScheduledStartLocal)
                    .NotNull()
                    .Must(value => value is not null && value.Value != default)
                    .WithMessage("Choose a start date and time.");
            });
        }
    }
}
